namespace VestaNET.Domain.Entities;
public class Ocorrencia
{
    public long Id { get; set; }
    public long AbrigoId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Severidade { get; set; } = string.Empty;
    public string Status { get; set; } = "ABERTA";
}
