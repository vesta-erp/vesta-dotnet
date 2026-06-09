using VestaNET.Domain.Enums;
namespace VestaNET.Domain.Entities;
public class Recomendacao
{
    public long Id { get; set; }
    public long AnaliseCriticidadeId { get; set; }
    public TipoRecomendacao Tipo { get; set; }
    public string Descricao { get; set; } = string.Empty;
}
