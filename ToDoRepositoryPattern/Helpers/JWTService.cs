using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ToDoRepositoryPattern.Models;

namespace ToDoRepositoryPattern.Helpers;

public class JWTService
{
    private readonly IConfiguration _configuration;
    public JWTService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public string GenerateToken(User user)
    {
        string ISSUER = _configuration.GetValue<string>("JWTSettings:ToDoAppIssuer")!;
        string AUDIENCE = _configuration.GetValue<string>("JWTSettings:ToDoAppAudience")!;
        string SECRET_KEY = _configuration.GetValue<string>("JWTSettings:SecretKey")!;

        DateTime EXPIRY = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("JWTSettings:ExpirationMinutes"));

        byte[] EncodedBytes = Encoding.UTF8.GetBytes(SECRET_KEY);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var SecurityKey = new SymmetricSecurityKey(EncodedBytes);

        var SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);

        var Token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken
        (
            issuer: ISSUER,
            audience: AUDIENCE,
            claims: claims,
            expires: EXPIRY,
            signingCredentials: SigningCredentials
        );

        string FinalToken = new JwtSecurityTokenHandler().WriteToken(Token);
        return FinalToken;
    }
}
