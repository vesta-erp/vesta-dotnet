using VestaNET.Application.Interfaces;
using VestaNET.Domain.Entities;
namespace VestaNET.Tests.Fakes;
public class FakeAnaliseRepository : IAnaliseRepository
{
    public List<AnaliseCriticidade> Salvas { get; } = new();
    public Task AdicionarAsync(AnaliseCriticidade a, CancellationToken ct = default) { Salvas.Add(a); return Task.CompletedTask; }
    public Task<List<AnaliseCriticidade>> ListarPorAbrigoAsync(long id, CancellationToken ct = default)
        => Task.FromResult(Salvas.Where(a => a.AbrigoId == id).ToList());
    public Task SalvarAsync(CancellationToken ct = default) => Task.CompletedTask;
}
