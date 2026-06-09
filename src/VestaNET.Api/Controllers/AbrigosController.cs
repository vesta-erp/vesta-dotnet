using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VestaNET.Application.DTOs;
using VestaNET.Application.Interfaces;
using VestaNET.Domain.Entities;

namespace VestaNET.Api.Controllers;

[ApiController]
[Route("api/abrigos")]
[Authorize]
[Produces("application/json")]
public class AbrigosController : ControllerBase
{
    private readonly IAbrigoRepository _repo;
    public AbrigosController(IAbrigoRepository repo) => _repo = repo;

    [HttpGet]
    [ProducesResponseType(typeof(List<AbrigoDto>), 200)]
    public async Task<ActionResult<List<AbrigoDto>>> Listar([FromQuery] long? regiaoId, CancellationToken ct)
        => Ok((await _repo.ListarComDetalhesAsync(regiaoId, ct)).Select(ToDto).ToList());

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(AbrigoDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<AbrigoDto>> Obter(long id, CancellationToken ct)
    {
        var a = await _repo.ObterComDetalhesAsync(id, ct);
        return a is null ? NotFound() : Ok(ToDto(a));
    }

    private static AbrigoDto ToDto(Abrigo a) => new(
        a.Id, a.Nome, a.Endereco,
        a.CapacidadeMaxima, a.OcupacaoAtual, Math.Round(a.TaxaOcupacao, 4), a.Status,
        a.OcorrenciasAbertas, a.SolicitacoesPendentes, a.RecursosAbaixoMinimo,
        a.Estoques.Select(e => new EstoqueDto(
            e.RecursoId, e.NomeRecurso, e.QuantidadeAtual, e.QuantidadeMinima, e.AbaixoDoMinimo
        )).ToList());
}
