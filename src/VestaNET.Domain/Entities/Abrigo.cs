namespace VestaNET.Domain.Entities;

public class Abrigo
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public int CapacidadeMaxima { get; set; }
    public int OcupacaoAtual { get; set; }
    public string Status { get; set; } = string.Empty;
    public long RegiaoId { get; set; }
    public long InstituicaoId { get; set; }

    public List<EstoqueAbrigo> Estoques { get; set; } = new();
    public List<Ocorrencia> Ocorrencias { get; set; } = new();
    public List<SolicitacaoRecurso> Solicitacoes { get; set; } = new();

    public double TaxaOcupacao =>
        CapacidadeMaxima > 0 ? (double)OcupacaoAtual / CapacidadeMaxima : 0;
    public int OcorrenciasAbertas =>
        Ocorrencias.Count(o => o.Status != "RESOLVIDA");
    public int SolicitacoesPendentes =>
        Solicitacoes.Count(s => s.Status is "ABERTA" or "EM_ANALISE" or "EM_ATENDIMENTO");
    public int RecursosAbaixoMinimo =>
        Estoques.Count(e => e.QuantidadeAtual < e.QuantidadeMinima);
}
