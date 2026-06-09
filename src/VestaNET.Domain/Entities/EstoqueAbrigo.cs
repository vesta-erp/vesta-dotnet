namespace VestaNET.Domain.Entities;
public class EstoqueAbrigo
{
    public long Id { get; set; }
    public long AbrigoId { get; set; }
    public long RecursoId { get; set; }
    public Recurso? Recurso { get; set; }
    public int QuantidadeAtual { get; set; }
    public int QuantidadeMinima { get; set; }
    public string NomeRecurso => Recurso?.Nome ?? $"Recurso #{RecursoId}";
    public bool AbaixoDoMinimo => QuantidadeAtual < QuantidadeMinima;
}
