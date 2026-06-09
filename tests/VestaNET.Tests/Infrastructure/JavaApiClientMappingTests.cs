using VestaNET.Infrastructure.HttpClients;
using Xunit;

namespace VestaNET.Tests.Infrastructure;

public class JavaApiClientMappingTests
{
    [Fact]
    public void MapToAbrigo_ComDadosCompletos_PopulaPropriedadesCorretamente()
    {
        var abrigoDto = new JavaAbrigoResponse(1, "Ginásio Municipal", "Rua A, 10",
            200, 150, "ATIVO", 1, 1);
        var estoques = new List<JavaEstoqueResponse>
        {
            new(10, 1, "Água Mineral", "litros", 50, 20)
        };
        var ocorrencias = new List<JavaOcorrenciaResponse>
        {
            new(5, "Infiltração", "ALTA", "ABERTA")
        };
        var solicitacoes = new List<JavaSolicitacaoResponse>
        {
            new(3, "ABERTA")
        };

        var abrigo = JavaApiMapper.ToAbrigo(abrigoDto, estoques, ocorrencias, solicitacoes);

        Assert.Equal(1, abrigo.Id);
        Assert.Equal("Ginásio Municipal", abrigo.Nome);
        Assert.Equal(200, abrigo.CapacidadeMaxima);
        Assert.Equal(150, abrigo.OcupacaoAtual);
        Assert.Equal("ATIVO", abrigo.Status);
        Assert.Single(abrigo.Estoques);
        Assert.Equal(50, abrigo.Estoques[0].QuantidadeAtual);
        Assert.Equal(20, abrigo.Estoques[0].QuantidadeMinima);
        Assert.False(abrigo.Estoques[0].AbaixoDoMinimo);
        Assert.Single(abrigo.Ocorrencias);
        Assert.Equal("ALTA", abrigo.Ocorrencias[0].Severidade);
        Assert.Single(abrigo.Solicitacoes);
        Assert.Equal("ABERTA", abrigo.Solicitacoes[0].Status);
    }

    [Fact]
    public void MapToAbrigo_EstoqueAbaixoMinimo_AbaixoDoMinimoEhTrue()
    {
        var abrigoDto = new JavaAbrigoResponse(1, "X", "Y", 100, 50, "ATIVO", 1, 1);
        var estoques = new List<JavaEstoqueResponse>
        {
            new(1, 1, "Comida", "kg", 5, 20)  // 5 < 20 → abaixo do mínimo
        };

        var abrigo = JavaApiMapper.ToAbrigo(abrigoDto, estoques, [], []);

        Assert.True(abrigo.Estoques[0].AbaixoDoMinimo);
    }
}
