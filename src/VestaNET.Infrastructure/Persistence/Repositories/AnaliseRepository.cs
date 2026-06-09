using Microsoft.EntityFrameworkCore;
using VestaNET.Application.Interfaces;
using VestaNET.Domain.Entities;

namespace VestaNET.Infrastructure.Persistence.Repositories;

public class AnaliseRepository : IAnaliseRepository
{
    private readonly AppDbContext _db;
    public AnaliseRepository(AppDbContext db) => _db = db;

    public async Task AdicionarAsync(AnaliseCriticidade a, CancellationToken ct = default)
        => await _db.Analises.AddAsync(a, ct);

    public async Task<List<AnaliseCriticidade>> ListarPorAbrigoAsync(long abrigoId, CancellationToken ct = default)
        => await _db.Analises.Include(a => a.Recomendacoes)
            .Where(a => a.AbrigoId == abrigoId)
            .OrderByDescending(a => a.DataAnalise)
            .AsNoTracking().ToListAsync(ct);

    public async Task SalvarAsync(CancellationToken ct = default)
        => await _db.SaveChangesAsync(ct);
}
