using VestaNET.Domain.Entities;
namespace VestaNET.Application.Interfaces;
public interface IAnaliseRepository
{
    Task AdicionarAsync(AnaliseCriticidade analise, CancellationToken ct = default);
    Task<List<AnaliseCriticidade>> ListarPorAbrigoAsync(long abrigoId, CancellationToken ct = default);
    Task SalvarAsync(CancellationToken ct = default);
}
