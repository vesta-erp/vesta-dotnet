using VestaNET.Application.Services;
using VestaNET.Domain.Entities;
using VestaNET.Domain.Enums;
using VestaNET.Domain.Services;
using VestaNET.Tests.Fakes;
using Xunit;

namespace VestaNET.Tests.Application;

public class AnaliseServiceTests
{
    private static Abrigo AbrigoCritico() => new()
    {
        Id = 1, Nome = "Ginásio", CapacidadeMaxima = 100, OcupacaoAtual = 100,
        Estoques     = [new EstoqueAbrigo { RecursoId = 1, QuantidadeAtual = 0, QuantidadeMinima = 100 }],
        Ocorrencias  = [new Ocorrencia { Severidade = "CRITICA", Status = "ABERTA" }],
        Solicitacoes = [new SolicitacaoRecurso { Status = "ABERTA" }]
    };

    [Fact]
    public async Task AnalisarAsync_DevePersistirERetornarAnaliseDetalhada()
    {
        var analises = new FakeAnaliseRepository();
        var svc = new AnaliseService(new FakeAbrigoRepository(AbrigoCritico()), analises, new CalculadoraCriticidade());
        var dto = await svc.AnalisarAsync(1);
        Assert.Equal("Critica", dto.Nivel);
        Assert.Single(analises.Salvas);
        Assert.NotEmpty(dto.Recomendacoes);
        Assert.NotEmpty(dto.DetalhesCalculo);
        Assert.Equal(4, dto.DetalhesCalculo.Count);
        Assert.False(string.IsNullOrEmpty(dto.ExplicacaoNivel));
        Assert.NotNull(dto.ParametrosBanco);
    }

    [Fact]
    public async Task AnalisarAsync_ParametrosBancoDevemRefletirValoresReais()
    {
        var abrigo = AbrigoCritico();
        var svc = new AnaliseService(new FakeAbrigoRepository(abrigo), new FakeAnaliseRepository(), new CalculadoraCriticidade());
        var dto = await svc.AnalisarAsync(1);
        Assert.Equal(100, dto.ParametrosBanco.CapacidadeMaxima);
        Assert.Equal(100, dto.ParametrosBanco.OcupacaoAtual);
        Assert.Equal(1,   dto.ParametrosBanco.TotalRecursos);
        Assert.Equal(1,   dto.ParametrosBanco.RecursosAbaixoMinimo);
        Assert.Equal(1,   dto.ParametrosBanco.TotalOcorrenciasAbertas);
        Assert.Equal(1,   dto.ParametrosBanco.OcorrenciasCriticas);
        Assert.Equal(1,   dto.ParametrosBanco.SolicitacoesPendentes);
    }

    [Fact]
    public async Task AnalisarAsync_AbrigoInexistente_DeveLancarKeyNotFound()
    {
        var svc = new AnaliseService(new FakeAbrigoRepository(), new FakeAnaliseRepository(), new CalculadoraCriticidade());
        await Assert.ThrowsAsync<KeyNotFoundException>(() => svc.AnalisarAsync(999));
    }

    [Fact]
    public async Task RankingAsync_DeveOrdenarPorScoreDescendente()
    {
        var critico = AbrigoCritico();
        var estavel = new Abrigo { Id = 2, Nome = "Centro", CapacidadeMaxima = 100, OcupacaoAtual = 10,
            Estoques = [new EstoqueAbrigo { QuantidadeAtual = 500, QuantidadeMinima = 100 }] };
        var svc = new AnaliseService(new FakeAbrigoRepository(critico, estavel),
            new FakeAnaliseRepository(), new CalculadoraCriticidade());
        var ranking = await svc.RankingAsync();
        Assert.Equal(2, ranking.Count);
        Assert.True(ranking[0].Score >= ranking[1].Score);
        Assert.Equal(1, ranking[0].Posicao);
    }
}
