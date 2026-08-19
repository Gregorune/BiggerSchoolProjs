using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FitnesAPI;

public class JwtHandler
{
    private readonly IConfiguration _config;
    public JwtHandler(IConfiguration config) => _config = config;
    public string CreateAccessToken(JwtContent content)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, content.Uid.ToString())
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public JwtContent GetJwtContent(ClaimsPrincipal claims)
    {
        return new JwtContent()
        {
            Uid = int.Parse(claims.FindFirstValue(ClaimTypes.NameIdentifier) ?? "-1")
        };
    }
    public struct JwtContent
    {
        public int Uid { get; set; }
    }
    
    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}