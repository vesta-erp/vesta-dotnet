using VestaNET.Domain.Entities;
namespace VestaNET.Application.Interfaces;
public interface IAbrigoRepository
{
    Task<List<Abrigo>> ListarComDetalhesAsync(long? regiaoId = null, CancellationToken ct = default);
    Task<Abrigo?> ObterComDetalhesAsync(long id, CancellationToken ct = default);
}
