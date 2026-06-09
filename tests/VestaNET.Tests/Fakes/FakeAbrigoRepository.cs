using VestaNET.Application.Interfaces;
using VestaNET.Domain.Entities;
namespace VestaNET.Tests.Fakes;
public class FakeAbrigoRepository : IAbrigoRepository
{
    private readonly List<Abrigo> _data;
    public FakeAbrigoRepository(params Abrigo[] abrigos) => _data = abrigos.ToList();
    public Task<Abrigo?> ObterComDetalhesAsync(long id, CancellationToken ct = default)
        => Task.FromResult(_data.FirstOrDefault(a => a.Id == id));
    public Task<List<Abrigo>> ListarComDetalhesAsync(long? regiaoId = null, CancellationToken ct = default)
        => Task.FromResult(_data);
}
