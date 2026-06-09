using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using VestaNET.Application.Settings;

namespace VestaNET.Infrastructure.HttpClients;

internal record JavaLoginRequest(string Email, string Senha);
internal record JavaLoginResponse(string Token, string Tipo);

internal class JavaAuthHandler : DelegatingHandler
{
    private readonly JavaApiSettings _settings;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private string? _token;

    public JavaAuthHandler(IOptions<JavaApiSettings> options)
    {
        _settings = options.Value;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(_token))
            await RefreshTokenAsync(ct);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _token);

        var response = await base.SendAsync(request, ct);

        // Token expirado: invalida cache para que a próxima requisição obtenha um novo token.
        // HttpRequestMessage não é reenviável após consumo — o caller deve retentar se necessário.
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            _token = null;

        return response;
    }

    private async Task RefreshTokenAsync(CancellationToken ct)
    {
        await _lock.WaitAsync(ct);
        try
        {
            if (!string.IsNullOrEmpty(_token)) return; // double-check dentro do lock
            _token = await ObterTokenAsync(ct);
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<string> ObterTokenAsync(CancellationToken ct)
    {
        using var loginClient = new HttpClient { BaseAddress = new Uri(_settings.BaseUrl) };
        var loginReq = new JavaLoginRequest(_settings.ServiceEmail, _settings.ServicePassword);
        var resp = await loginClient.PostAsJsonAsync("api/auth/login", loginReq, ct);
        resp.EnsureSuccessStatusCode();
        var body = await resp.Content.ReadFromJsonAsync<JavaLoginResponse>(ct)
            ?? throw new InvalidOperationException("Java API retornou token nulo.");
        return body.Token;
    }
}
