using Microsoft.AspNetCore.Mvc;
using VestaNET.Application.DTOs;
using VestaNET.Infrastructure.Security;

namespace VestaNET.Api.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly JwtTokenGenerator _jwt;
    public AuthController(JwtTokenGenerator jwt) => _jwt = jwt;

    [HttpPost("token")]
    [ProducesResponseType(typeof(TokenResponse), 200)]
    public ActionResult<TokenResponse> GerarToken([FromBody] TokenRequest req)
    {
        var role = req.Role?.ToUpper() switch
        {
            "ADMIN"  => "ADMIN",
            "GESTOR" => "GESTOR",
            _        => "OPERADOR"
        };
        var (token, expira) = _jwt.Gerar(req.UserId ?? "demo", req.Email ?? "demo@vesta.gov.br", role);
        return Ok(new TokenResponse(token, expira, role));
    }
}
