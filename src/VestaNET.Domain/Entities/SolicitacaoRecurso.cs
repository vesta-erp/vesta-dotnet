namespace VestaNET.Domain.Entities;
public class SolicitacaoRecurso
{
    public long Id { get; set; }
    public long AbrigoId { get; set; }
    public long RecursoId { get; set; }
    public int Quantidade { get; set; }
    public string Status { get; set; } = "ABERTA";
}
