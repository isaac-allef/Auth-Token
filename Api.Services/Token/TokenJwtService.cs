using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace Api.Services;

public class TokenJwtService : ITokenService
{
    private readonly Settings _settings;

    public TokenJwtService(Settings settings)
    {
        _settings = settings;
    }

    public Token Generate(string username, int expiresInMinutes)
    {
        var secretKey = Encoding.ASCII.GetBytes(_settings.Secret);
        var expiresIn = DateTime.UtcNow.AddMinutes(expiresInMinutes);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new []
            {
                new Claim(ClaimTypes.Name, username)
            }),
            Expires = expiresIn,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(secretKey),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new Token()
        {
            Value = tokenHandler.WriteToken(token),
            ExpiresIn = expiresIn
        };
    }
}
