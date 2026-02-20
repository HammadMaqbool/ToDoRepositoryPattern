using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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
        string ISSUER = _configuration.GetValue<string>("JWTSettings:Issuer")!;
        string AUDIENCE = _configuration.GetValue<string>("JWTSettings:Audience")!;
        string SECRET_KEY = _configuration.GetValue<string>("JWTSettings:SecretKey")!;

        DateTime EXPIRY = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("JWTSettings:ExpirationMinutes"));

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

    public static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
