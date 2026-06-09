using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using VestaNET.Application.Settings;

namespace VestaNET.Infrastructure.HttpClients;

// TODO: Remove these inline stubs once Task 4 (JavaApiResponseDtos.cs) is merged.
// Duplicated from JavaApiResponseDtos.cs — will merge when Task 4 completes.
internal record JavaLoginRequest(string Email, string Senha);
internal record JavaLoginResponse(string Token, string Tipo);

internal class JavaAuthHandler : DelegatingHandler
{
    private readonly JavaApiSettings _settings;
    private string? _token;

    public JavaAuthHandler(IOptions<JavaApiSettings> options)
    {
        _settings = options.Value;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(_token))
            _token = await ObterTokenAsync(ct);

        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

        var response = await base.SendAsync(request, ct);

        // Token expirado: obtém novo token e tenta uma vez
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _token = await ObterTokenAsync(ct);
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            response = await base.SendAsync(request, ct);
        }

        return response;
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
