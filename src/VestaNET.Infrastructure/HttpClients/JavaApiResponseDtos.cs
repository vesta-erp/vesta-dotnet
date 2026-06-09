using System.Text.Json.Serialization;
using VestaNET.Domain.Entities;

namespace VestaNET.Infrastructure.HttpClients;

// --- DTOs das respostas da Java API ---

internal record JavaAbrigoResponse(
    [property: JsonPropertyName("idAbrigo")] long IdAbrigo,
    [property: JsonPropertyName("nmAbrigo")] string NmAbrigo,
    [property: JsonPropertyName("dsEndereco")] string DsEndereco,
    [property: JsonPropertyName("qtCapacidadeMaxima")] int QtCapacidadeMaxima,
    [property: JsonPropertyName("qtOcupacaoAtual")] int QtOcupacaoAtual,
    [property: JsonPropertyName("stStatus")] string StStatus,
    [property: JsonPropertyName("idRegiao")] long IdRegiao,
    [property: JsonPropertyName("idInstituicao")] long IdInstituicao);

internal record JavaEstoqueResponse(
    [property: JsonPropertyName("idEstoque")] long IdEstoque,
    [property: JsonPropertyName("idRecurso")] long IdRecurso,
    [property: JsonPropertyName("nmRecurso")] string NmRecurso,
    [property: JsonPropertyName("dsUnidadeMedida")] string DsUnidadeMedida,
    [property: JsonPropertyName("qtAtual")] int QtAtual,
    [property: JsonPropertyName("qtMinima")] int QtMinima);

internal record JavaOcorrenciaResponse(
    [property: JsonPropertyName("idOcorrencia")] long IdOcorrencia,
    [property: JsonPropertyName("nmTitulo")] string NmTitulo,
    [property: JsonPropertyName("tpSeveridade")] string TpSeveridade,
    [property: JsonPropertyName("stStatus")] string StStatus);

internal record JavaSolicitacaoResponse(
    [property: JsonPropertyName("idSolicitacao")] long IdSolicitacao,
    [property: JsonPropertyName("stStatus")] string StStatus);

// --- Wrappers HATEOAS para listas ---

internal record JavaEstoqueHateoas(
    [property: JsonPropertyName("_embedded")] JavaEstoqueEmbedded? Embedded);
internal record JavaEstoqueEmbedded(
    [property: JsonPropertyName("estoqueResponseList")] List<JavaEstoqueResponse>? Items);

internal record JavaOcorrenciaHateoas(
    [property: JsonPropertyName("_embedded")] JavaOcorrenciaEmbedded? Embedded);
internal record JavaOcorrenciaEmbedded(
    [property: JsonPropertyName("ocorrenciaResponseList")] List<JavaOcorrenciaResponse>? Items);

internal record JavaSolicitacaoHateoas(
    [property: JsonPropertyName("_embedded")] JavaSolicitacaoEmbedded? Embedded);
internal record JavaSolicitacaoEmbedded(
    [property: JsonPropertyName("solicitacaoResponseList")] List<JavaSolicitacaoResponse>? Items);

internal record JavaAbrigoHateoas(
    [property: JsonPropertyName("_embedded")] JavaAbrigoEmbedded? Embedded);
internal record JavaAbrigoEmbedded(
    [property: JsonPropertyName("abrigoResponseList")] List<JavaAbrigoResponse>? Items);

// --- Mapper ---

internal static class JavaApiMapper
{
    public static Abrigo ToAbrigo(
        JavaAbrigoResponse a,
        List<JavaEstoqueResponse> estoques,
        List<JavaOcorrenciaResponse> ocorrencias,
        List<JavaSolicitacaoResponse> solicitacoes) => new()
    {
        Id = a.IdAbrigo,
        Nome = a.NmAbrigo,
        Endereco = a.DsEndereco,
        CapacidadeMaxima = a.QtCapacidadeMaxima,
        OcupacaoAtual = a.QtOcupacaoAtual,
        Status = a.StStatus,
        RegiaoId = a.IdRegiao,
        InstituicaoId = a.IdInstituicao,
        Estoques = estoques.Select(e => new EstoqueAbrigo
        {
            Id = e.IdEstoque,
            RecursoId = e.IdRecurso,
            QuantidadeAtual = e.QtAtual,
            QuantidadeMinima = e.QtMinima,
            Recurso = new Recurso { Id = e.IdRecurso, Nome = e.NmRecurso, UnidadeMedida = e.DsUnidadeMedida }
        }).ToList(),
        Ocorrencias = ocorrencias.Select(o => new Ocorrencia
        {
            Id = o.IdOcorrencia,
            Titulo = o.NmTitulo,
            Severidade = o.TpSeveridade,
            Status = o.StStatus
        }).ToList(),
        Solicitacoes = solicitacoes.Select(s => new SolicitacaoRecurso
        {
            Id = s.IdSolicitacao,
            Status = s.StStatus
        }).ToList()
    };
}
