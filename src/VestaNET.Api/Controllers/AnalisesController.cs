using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VestaNET.Application.DTOs;
using VestaNET.Application.Services;

namespace VestaNET.Api.Controllers;

[ApiController]
[Route("api/analises")]
[Authorize]
[Produces("application/json")]
public class AnalisesController : ControllerBase
{
    private readonly AnaliseService _svc;
    public AnalisesController(AnaliseService svc) => _svc = svc;

    [HttpPost("abrigos/{abrigoId:long}")]
    [ProducesResponseType(typeof(AnaliseDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<AnaliseDto>> Analisar(long abrigoId, CancellationToken ct)
        => Ok(await _svc.AnalisarAsync(abrigoId, ct));

    [HttpGet("abrigos/{abrigoId:long}/historico")]
    [ProducesResponseType(typeof(List<AnaliseDto>), 200)]
    public async Task<ActionResult<List<AnaliseDto>>> Historico(long abrigoId, CancellationToken ct)
        => Ok(await _svc.HistoricoAsync(abrigoId, ct));

    [HttpGet("ranking")]
    [ProducesResponseType(typeof(List<RankingItemDto>), 200)]
    public async Task<ActionResult<List<RankingItemDto>>> Ranking([FromQuery] long? regiaoId, CancellationToken ct)
        => Ok(await _svc.RankingAsync(regiaoId, ct));
}
