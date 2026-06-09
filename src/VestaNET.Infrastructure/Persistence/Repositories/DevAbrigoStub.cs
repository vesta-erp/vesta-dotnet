using VestaNET.Application.Interfaces;
using VestaNET.Domain.Entities;

namespace VestaNET.Infrastructure.Persistence.Repositories;

internal class DevAbrigoStub : IJavaApiClient
{
    private static readonly List<Abrigo> _data =
    [
        new()
        {
            Id = 1, Nome = "Ginásio Municipal (stub)", Endereco = "Rua A, 100",
            CapacidadeMaxima = 200, OcupacaoAtual = 195, Status = "LOTADO", RegiaoId = 1, InstituicaoId = 1,
            Estoques =
            [
                new() { Id = 1, AbrigoId = 1, RecursoId = 1, QuantidadeAtual = 5, QuantidadeMinima = 20,
                    Recurso = new() { Id = 1, Nome = "Água Mineral", UnidadeMedida = "L" } }
            ],
            Ocorrencias =
            [
                new() { Id = 1, AbrigoId = 1, Titulo = "Superlotação", Severidade = "CRITICA", Status = "ABERTA" }
            ],
            Solicitacoes =
            [
                new() { Id = 1, AbrigoId = 1, Status = "ABERTA" }
            ]
        },
        new()
        {
            Id = 2, Nome = "Escola Estadual (stub)", Endereco = "Av. B, 200",
            CapacidadeMaxima = 150, OcupacaoAtual = 60, Status = "ATIVO", RegiaoId = 1, InstituicaoId = 1,
            Estoques =
            [
                new() { Id = 2, AbrigoId = 2, RecursoId = 2, QuantidadeAtual = 100, QuantidadeMinima = 30,
                    Recurso = new() { Id = 2, Nome = "Alimento", UnidadeMedida = "kg" } }
            ],
            Ocorrencias = [],
            Solicitacoes = []
        }
    ];

    public Task<Abrigo?> ObterComDetalhesAsync(long id, CancellationToken ct = default) =>
        Task.FromResult(_data.FirstOrDefault(a => a.Id == id));

    public Task<List<Abrigo>> ListarComDetalhesAsync(long? regiaoId = null, CancellationToken ct = default) =>
        Task.FromResult(_data.ToList());
}
