using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using slimeMaster_server.Models;

namespace slimeMaster_server.Services;

public class AuthService
{
    private string _jwtSecret;

    public AuthService()
    {
        string jsonData = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/config.json"));
        var configData = JsonConvert.DeserializeObject<ConfigData>(jsonData);
        _jwtSecret = configData.jwtKey;
    }
    
    public string GenerateJwtToken(string uid)
    {
        long now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        long expired = now + 3600 * 24 * 1000;

        var claims = new Claim[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, uid), // Subject
            new Claim(JwtRegisteredClaimNames.Iat, now.ToString()), // Issued At
            new Claim(JwtRegisteredClaimNames.Exp, expired.ToString()) // Expiration
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret)),
            SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(claims: claims, signingCredentials: credentials);
        string token = new JwtSecurityTokenHandler().WriteToken(jwt);

        return token;
    }

    public ClaimsPrincipal ValidateJwtToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSecret);
        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            }, out SecurityToken validatedToken);
            
            return principal;
        }
        catch
        {
            return null;
        }
    }
}
