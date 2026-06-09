using Microsoft.AspNetCore.Mvc;
using VestaNET.Application.DTOs;
using VestaNET.Application.Services;

namespace VestaNET.Api.Controllers;

[ApiController]
[Route("api/criticidade")]
[Produces("application/json")]
public class CriticidadeController(AnaliseService analiseService) : ControllerBase
{
    [HttpGet("abrigos")]
    [ProducesResponseType(typeof(List<CriticidadeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] long? regiaoId, CancellationToken ct) =>
        Ok(await analiseService.CalcularTodosAsync(regiaoId, ct));

    [HttpGet("abrigos/{id:long}")]
    [ProducesResponseType(typeof(CriticidadeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Buscar(long id, CancellationToken ct)
    {
        var result = await analiseService.CalcularAsync(id, ct);
        return result is null ? NotFound() : Ok(result);
    }
}
