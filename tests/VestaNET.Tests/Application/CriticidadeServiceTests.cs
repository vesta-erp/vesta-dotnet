using VestaNET.Application.DTOs;
using VestaNET.Application.Services;
using VestaNET.Domain.Entities;
using VestaNET.Tests.Fakes;
using Xunit;

namespace VestaNET.Tests.Application;

public class CriticidadeServiceTests
{
    private static AnaliseService CriarService(params Abrigo[] abrigos)
    {
        var abrigoRepo = new FakeAbrigoRepository(abrigos);
        var analiseRepo = new FakeAnaliseRepository();
        var calculadora = new VestaNET.Domain.Services.CalculadoraCriticidade();
        return new AnaliseService(abrigoRepo, analiseRepo, calculadora);
    }

    [Fact]
    public async Task CalcularAsync_AbrigoExistente_RetornaCriticidadeDto()
    {
        var abrigo = new Abrigo
        {
            Id = 1, Nome = "Abrigo Teste", CapacidadeMaxima = 100, OcupacaoAtual = 95,
            Status = "ATIVO",
            Ocorrencias = [new() { Id = 1, Severidade = "CRITICA", Status = "ABERTA" }],
            Estoques = [], Solicitacoes = []
        };
        var service = CriarService(abrigo);

        var result = await service.CalcularAsync(1, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result.IdAbrigo);
        Assert.True(result.Score > 0);
        Assert.NotEmpty(result.Nivel);
        Assert.NotEmpty(result.Justificativa);
    }

    [Fact]
    public async Task CalcularAsync_AbrigoInexistente_RetornaNull()
    {
        var service = CriarService();

        var result = await service.CalcularAsync(999, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task CalcularTodosAsync_RetornaUmItemPorAbrigo()
    {
        var a1 = new Abrigo { Id = 1, Nome = "A1", CapacidadeMaxima = 100, OcupacaoAtual = 90, Status = "ATIVO", Ocorrencias = [], Estoques = [], Solicitacoes = [] };
        var a2 = new Abrigo { Id = 2, Nome = "A2", CapacidadeMaxima = 100, OcupacaoAtual = 20, Status = "ATIVO", Ocorrencias = [], Estoques = [], Solicitacoes = [] };
        var service = CriarService(a1, a2);

        var result = await service.CalcularTodosAsync(null, CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.IdAbrigo == 1);
        Assert.Contains(result, r => r.IdAbrigo == 2);
    }
}
