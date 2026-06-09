using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace VestaNET.Infrastructure.Security;

public class JwtTokenGenerator
{
    private readonly JwtSettings _cfg;
    public JwtTokenGenerator(IOptions<JwtSettings> cfg) => _cfg = cfg.Value;

    public (string Token, DateTime Expira) Gerar(string userId, string email, string role)
    {
        var expira = DateTime.UtcNow.AddMinutes(_cfg.ExpiracaoMinutos);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var key  = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg.SecretKey));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(_cfg.Issuer, _cfg.Audience, claims,
            expires: expira, signingCredentials: cred);
        return (new JwtSecurityTokenHandler().WriteToken(token), expira);
    }
}
