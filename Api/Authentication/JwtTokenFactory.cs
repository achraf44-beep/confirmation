using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Api.Authentication;

public sealed class JwtTokenFactory
{
    private readonly JwtOptions options;

    public JwtTokenFactory(IOptions<JwtOptions> options)
    {
        this.options = options.Value;
    }

    public (string Token, DateTime ExpiresAtUtc) CreateToken(AuthenticatedUserDto user)
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(options.ExpiryMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        claims.AddRange(user.Claims.Select(claim => new Claim(claim.Type, claim.Value)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
    }
}