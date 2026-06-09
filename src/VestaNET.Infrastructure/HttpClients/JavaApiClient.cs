using System.Net;
using System.Net.Http.Json;
using VestaNET.Application.Interfaces;
using VestaNET.Domain.Entities;

namespace VestaNET.Infrastructure.HttpClients;

public class JavaApiClient(HttpClient http) : IJavaApiClient
{
    public async Task<Abrigo?> ObterComDetalhesAsync(long id, CancellationToken ct = default)
    {
        var abrigoResp = await http.GetAsync($"api/abrigos/{id}", ct);
        if (abrigoResp.StatusCode == HttpStatusCode.NotFound) return null;
        abrigoResp.EnsureSuccessStatusCode();

        var abrigo = await abrigoResp.Content.ReadFromJsonAsync<JavaAbrigoResponse>(ct)
                     ?? throw new InvalidOperationException($"Abrigo {id} retornou corpo nulo.");

        var estTask = ObterEstoquesAsync(id, ct);
        var ocTask  = ObterOcorrenciasAsync(id, ct);
        var solTask = ObterSolicitacoesAsync(id, ct);
        await Task.WhenAll(estTask, ocTask, solTask);

        return JavaApiMapper.ToAbrigo(abrigo, await estTask, await ocTask, await solTask);
    }

    public async Task<List<Abrigo>> ListarComDetalhesAsync(long? regiaoId = null, CancellationToken ct = default)
    {
        var url = regiaoId.HasValue ? $"api/abrigos?idRegiao={regiaoId}" : "api/abrigos";
        var hateoas = await http.GetFromJsonAsync<JavaAbrigoHateoas>(url, ct);
        var abrigosBasicos = hateoas?.Embedded?.Items ?? [];

        var tarefas = abrigosBasicos.Select(a => ObterComDetalhesAsync(a.IdAbrigo, ct));
        var resultados = await Task.WhenAll(tarefas);
        return resultados.OfType<Abrigo>().ToList();
    }

    private async Task<List<JavaEstoqueResponse>> ObterEstoquesAsync(long id, CancellationToken ct)
    {
        var resp = await http.GetAsync($"api/abrigos/{id}/estoque", ct);
        if (resp.StatusCode == HttpStatusCode.NotFound) return [];
        resp.EnsureSuccessStatusCode();
        var hateoas = await resp.Content.ReadFromJsonAsync<JavaEstoqueHateoas>(ct);
        return hateoas?.Embedded?.Items ?? [];
    }

    private async Task<List<JavaOcorrenciaResponse>> ObterOcorrenciasAsync(long id, CancellationToken ct)
    {
        var resp = await http.GetAsync($"api/abrigos/{id}/ocorrencias", ct);
        if (resp.StatusCode == HttpStatusCode.NotFound) return [];
        resp.EnsureSuccessStatusCode();
        var hateoas = await resp.Content.ReadFromJsonAsync<JavaOcorrenciaHateoas>(ct);
        return hateoas?.Embedded?.Items ?? [];
    }

    private async Task<List<JavaSolicitacaoResponse>> ObterSolicitacoesAsync(long id, CancellationToken ct)
    {
        var resp = await http.GetAsync($"api/abrigos/{id}/solicitacoes", ct);
        if (resp.StatusCode == HttpStatusCode.NotFound) return [];
        resp.EnsureSuccessStatusCode();
        var hateoas = await resp.Content.ReadFromJsonAsync<JavaSolicitacaoHateoas>(ct);
        return hateoas?.Embedded?.Items ?? [];
    }
}
