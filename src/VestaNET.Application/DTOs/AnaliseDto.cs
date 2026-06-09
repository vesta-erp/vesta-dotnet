namespace VestaNET.Application.DTOs;

public record RecomendacaoDto(string Tipo, string Descricao);

public record FatorCalculoDto(
    string Fator,
    double Peso,
    double ValorBruto,
    double ScoreParcial,
    string Descricao);

public record ParametrosBancoDto(
    long AbrigoId,
    string NomeAbrigo,
    int CapacidadeMaxima,
    int OcupacaoAtual,
    double TaxaOcupacao,
    int TotalRecursos,
    int RecursosAbaixoMinimo,
    List<string> RecursosCriticos,
    int TotalOcorrenciasAbertas,
    int OcorrenciasCriticas,
    int OcorrenciasAltas,
    int OcorrenciasMedias,
    int OcorrenciasBaixas,
    int SolicitacoesPendentes);

public record AnaliseDto(
    long Id,
    long AbrigoId,
    string NomeAbrigo,
    double Score,
    string Nivel,
    string ExplicacaoNivel,
    ParametrosBancoDto ParametrosBanco,
    List<FatorCalculoDto> DetalhesCalculo,
    List<string> Justificativas,
    List<RecomendacaoDto> Recomendacoes,
    DateTime DataAnalise);

public record RankingItemDto(
    int Posicao,
    long AbrigoId,
    string Nome,
    double Score,
    string Nivel,
    double TaxaOcupacao,
    int OcorrenciasAbertas,
    int SolicitacoesPendentes);
