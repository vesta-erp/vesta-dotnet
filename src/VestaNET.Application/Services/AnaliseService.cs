using VestaNET.Application.DTOs;
using VestaNET.Application.Interfaces;
using VestaNET.Domain.Entities;
using VestaNET.Domain.Services;

namespace VestaNET.Application.Services;

public class AnaliseService
{
    private readonly IAbrigoRepository _abrigos;
    private readonly IAnaliseRepository _analises;
    private readonly CalculadoraCriticidade _calc;

    public AnaliseService(IAbrigoRepository abrigos, IAnaliseRepository analises, CalculadoraCriticidade calc)
    {
        _abrigos = abrigos; _analises = analises; _calc = calc;
    }

    public async Task<AnaliseDto> AnalisarAsync(long abrigoId, CancellationToken ct = default)
    {
        var abrigo = await _abrigos.ObterComDetalhesAsync(abrigoId, ct)
            ?? throw new KeyNotFoundException($"Abrigo {abrigoId} não encontrado.");

        var r = _calc.Calcular(abrigo);

        var analise = new AnaliseCriticidade
        {
            AbrigoId      = abrigo.Id,
            Score         = r.Score,
            Nivel         = r.Nivel,
            Justificativa = string.Join('\n', r.Justificativas),
            DataAnalise   = DateTime.UtcNow,
            Recomendacoes = r.Recomendacoes
                .Select(x => new Recomendacao { Tipo = x.Tipo, Descricao = x.Descricao })
                .ToList()
        };

        await _analises.AdicionarAsync(analise, ct);
        await _analises.SalvarAsync(ct);
        return ToDto(analise, abrigo.Nome, r);
    }

    public async Task<List<AnaliseDto>> HistoricoAsync(long abrigoId, CancellationToken ct = default)
    {
        var abrigo = await _abrigos.ObterComDetalhesAsync(abrigoId, ct)
            ?? throw new KeyNotFoundException($"Abrigo {abrigoId} não encontrado.");
        var lista = await _analises.ListarPorAbrigoAsync(abrigoId, ct);
        var r = _calc.Calcular(abrigo);
        return lista.Select(a => ToDto(a, abrigo.Nome, r)).ToList();
    }

    public async Task<List<RankingItemDto>> RankingAsync(long? regiaoId = null, CancellationToken ct = default)
    {
        var abrigos = await _abrigos.ListarComDetalhesAsync(regiaoId, ct);
        return abrigos
            .Select(a => new { a, r = _calc.Calcular(a) })
            .OrderByDescending(x => x.r.Score)
            .Select((x, i) => new RankingItemDto(
                i + 1, x.a.Id, x.a.Nome,
                x.r.Score, x.r.Nivel.ToString(),
                Math.Round(x.a.TaxaOcupacao, 4),
                x.a.OcorrenciasAbertas, x.a.SolicitacoesPendentes))
            .ToList();
    }

    public async Task<CriticidadeDto?> CalcularAsync(long abrigoId, CancellationToken ct = default)
    {
        var abrigo = await _abrigos.ObterComDetalhesAsync(abrigoId, ct);
        if (abrigo == null) return null;
        var resultado = _calc.Calcular(abrigo);
        return ToDto(abrigo.Id, resultado);
    }

    public async Task<List<CriticidadeDto>> CalcularTodosAsync(long? regiaoId, CancellationToken ct = default)
    {
        var abrigos = await _abrigos.ListarComDetalhesAsync(regiaoId, ct);
        return abrigos
            .Select(a => ToDto(a.Id, _calc.Calcular(a)))
            .ToList();
    }

    private static CriticidadeDto ToDto(long id, ResultadoCriticidade r) =>
        new(
            id,
            (int)Math.Round(r.Score),
            r.Nivel.ToString().ToUpper(),
            string.Join("; ", r.Justificativas),
            r.Recomendacoes.Select(rec => rec.Descricao).ToList());

    private static AnaliseDto ToDto(AnaliseCriticidade a, string nomeAbrigo, ResultadoCriticidade r) => new(
        a.Id,
        a.AbrigoId,
        nomeAbrigo,
        a.Score,
        a.Nivel.ToString(),
        r.ExplicacaoNivel,
        new ParametrosBancoDto(
            a.AbrigoId, nomeAbrigo,
            r.CapacidadeMaxima, r.OcupacaoAtual,
            r.CapacidadeMaxima > 0 ? Math.Round((double)r.OcupacaoAtual / r.CapacidadeMaxima, 4) : 0,
            r.TotalRecursos, r.RecursosAbaixoMinimo, r.RecursosCriticos,
            r.TotalOcorrenciasAbertas, r.OcorrenciasCriticas, r.OcorrenciasAltas,
            r.OcorrenciasMedias, r.OcorrenciasBaixas, r.SolicitacoesPendentes),
        r.Fatores.Select(f => new FatorCalculoDto(
            f.Fator, f.Peso, f.ValorBruto, f.ScoreParcial, f.Descricao)).ToList(),
        a.Justificativa.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(),
        a.Recomendacoes.Select(rec => new RecomendacaoDto(rec.Tipo.ToString(), rec.Descricao)).ToList(),
        a.DataAnalise);
}
