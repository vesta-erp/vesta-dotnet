using Microsoft.EntityFrameworkCore;
using VestaNET.Application.Interfaces;
using VestaNET.Domain.Entities;

namespace VestaNET.Infrastructure.Persistence.Repositories;

public class AbrigoRepository : IAbrigoRepository
{
    private readonly AppDbContext _db;
    public AbrigoRepository(AppDbContext db) => _db = db;

    public async Task<Abrigo?> ObterComDetalhesAsync(long id, CancellationToken ct = default)
    {
        var abrigo = await _db.Abrigos.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, ct);
        if (abrigo is null) return null;
        await CarregarDetalhesAsync(abrigo, ct);
        return abrigo;
    }

    public async Task<List<Abrigo>> ListarComDetalhesAsync(long? regiaoId = null, CancellationToken ct = default)
    {
        var q = _db.Abrigos.AsNoTracking().AsQueryable();
        if (regiaoId.HasValue) q = q.Where(a => a.RegiaoId == regiaoId.Value);
        var lista = await q.ToListAsync(ct);
        if (!lista.Any()) return lista;

        var ids = lista.Select(a => a.Id).ToList();

        var estoques = await _db.Estoques.Include(e => e.Recurso)
            .Where(e => ids.Contains(e.AbrigoId)).AsNoTracking().ToListAsync(ct);
        var ocorrencias = await _db.Ocorrencias
            .Where(o => ids.Contains(o.AbrigoId)).AsNoTracking().ToListAsync(ct);
        var solicitacoes = await _db.Solicitacoes
            .Where(s => ids.Contains(s.AbrigoId)).AsNoTracking().ToListAsync(ct);

        foreach (var a in lista)
        {
            a.Estoques     = estoques.Where(e => e.AbrigoId == a.Id).ToList();
            a.Ocorrencias  = ocorrencias.Where(o => o.AbrigoId == a.Id).ToList();
            a.Solicitacoes = solicitacoes.Where(s => s.AbrigoId == a.Id).ToList();
        }
        return lista;
    }

    private async Task CarregarDetalhesAsync(Abrigo a, CancellationToken ct)
    {
        a.Estoques     = await _db.Estoques.Include(e => e.Recurso)
            .Where(e => e.AbrigoId == a.Id).AsNoTracking().ToListAsync(ct);
        a.Ocorrencias  = await _db.Ocorrencias
            .Where(o => o.AbrigoId == a.Id).AsNoTracking().ToListAsync(ct);
        a.Solicitacoes = await _db.Solicitacoes
            .Where(s => s.AbrigoId == a.Id).AsNoTracking().ToListAsync(ct);
    }
}
