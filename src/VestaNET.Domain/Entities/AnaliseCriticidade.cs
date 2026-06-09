using VestaNET.Domain.Enums;
namespace VestaNET.Domain.Entities;

public class AnaliseCriticidade
{
    public long Id { get; set; }
    public long AbrigoId { get; set; }
    public double Score { get; set; }
    public NivelCriticidade Nivel { get; set; }
    public string Justificativa { get; set; } = string.Empty;
    public DateTime DataAnalise { get; set; } = DateTime.UtcNow;
    public List<Recomendacao> Recomendacoes { get; set; } = new();
}
