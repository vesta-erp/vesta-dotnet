namespace VestaNET.Application.DTOs;
public record EstoqueDto(long RecursoId, string Recurso, int Atual, int Minimo, bool AbaixoDoMinimo);
public record AbrigoDto(
    long Id, string Nome, string Endereco,
    int CapacidadeMaxima, int OcupacaoAtual, double TaxaOcupacao, string Status,
    int OcorrenciasAbertas, int SolicitacoesPendentes, int RecursosAbaixoMinimo,
    List<EstoqueDto> Estoques);
