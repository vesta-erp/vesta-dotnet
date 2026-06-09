using VestaNET.Domain.Entities;
using VestaNET.Domain.Enums;
using VestaNET.Domain.Services;
using Xunit;

namespace VestaNET.Tests.Domain;

public class CalculadoraCriticidadeTests
{
    private readonly CalculadoraCriticidade _calc = new();

    private static Abrigo Criar(int cap, int ocup,
        (int atual, int min)[]? estoques = null,
        string[]? severidades = null,
        int solicitacoes = 0)
    {
        var a = new Abrigo { Id = 1, Nome = "Teste", CapacidadeMaxima = cap, OcupacaoAtual = ocup };
        foreach (var e in estoques ?? [])
            a.Estoques.Add(new EstoqueAbrigo { RecursoId = 1, QuantidadeAtual = e.atual, QuantidadeMinima = e.min });
        foreach (var s in severidades ?? [])
            a.Ocorrencias.Add(new Ocorrencia { Severidade = s, Status = "ABERTA" });
        for (int i = 0; i < solicitacoes; i++)
            a.Solicitacoes.Add(new SolicitacaoRecurso { Status = "ABERTA" });
        return a;
    }

    [Fact]
    public void AbrigoLotadoComProblemasGraves_DeveRetornarCritica()
    {
        var a = Criar(100, 100, [(0, 100)], ["CRITICA"], 3);
        var r = _calc.Calcular(a);
        Assert.Equal(NivelCriticidade.Critica, r.Nivel);
        Assert.True(r.Score >= 75);
    }

    [Fact]
    public void AbrigoEstavel_DeveRetornarBaixa()
    {
        var a = Criar(100, 10, [(500, 100)]);
        var r = _calc.Calcular(a);
        Assert.Equal(NivelCriticidade.Baixa, r.Nivel);
        Assert.True(r.Score < 25);
    }

    [Fact]
    public void RecursoAbaixoDoMinimo_GeraRecomendacaoReposicao()
    {
        var a = Criar(100, 30, [(5, 100)]);
        var r = _calc.Calcular(a);
        Assert.Contains(r.Recomendacoes, x => x.Tipo == TipoRecomendacao.Reposicao);
    }

    [Fact]
    public void OcupacaoAcima90_GeraRecomendacaoTransferencia()
    {
        var a = Criar(100, 92);
        var r = _calc.Calcular(a);
        Assert.Contains(r.Recomendacoes, x => x.Tipo == TipoRecomendacao.Transferencia);
    }

    [Fact]
    public void RetornoDeveConterQuatroFatoresDeCalculo()
    {
        var a = Criar(100, 80, [(50, 100)], ["ALTA"], 2);
        var r = _calc.Calcular(a);
        Assert.Equal(4, r.Fatores.Count);
        Assert.Contains(r.Fatores, f => f.Fator == "Ocupação");
        Assert.Contains(r.Fatores, f => f.Fator == "Recursos Abaixo do Mínimo");
        Assert.Contains(r.Fatores, f => f.Fator == "Ocorrências Abertas");
        Assert.Contains(r.Fatores, f => f.Fator == "Solicitações Pendentes");
    }

    [Fact]
    public void ExplicacaoNivelDeveSerPreenchida()
    {
        var a = Criar(100, 95, [(0, 100)], ["CRITICA"]);
        var r = _calc.Calcular(a);
        Assert.False(string.IsNullOrEmpty(r.ExplicacaoNivel));
        Assert.Contains(r.Score.ToString("F0"), r.ExplicacaoNivel);
    }

    [Fact]
    public void SomaDosScoresParciais_DeveIgualarScoreTotal()
    {
        var a = Criar(100, 75, [(30, 100)], ["ALTA"], 1);
        var r = _calc.Calcular(a);
        var somaFatores = Math.Round(r.Fatores.Sum(f => f.ScoreParcial), 2);
        Assert.Equal(r.Score, somaFatores);
    }
}
