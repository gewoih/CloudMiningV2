using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CloudMining.Domain.Models.Identity;
using CloudMining.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CloudMining.Application.Services.JWT;

public class JwtService
{
    private readonly string _signingKey;
    private readonly int _jwtLifetimeInDays;

    public JwtService(IOptions<JwtSettings> settings)
    {
        _signingKey = settings.Value.SigningKey;
        _jwtLifetimeInDays = settings.Value.LifetimeInDays;
    }

    public string GetSubClaim(string token)
    {
        if (new JwtSecurityTokenHandler().ReadToken(token) is not JwtSecurityToken jwt) 
            return null;
	        
        var sub = jwt.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value;
        return sub;
    }
    
    public string Generate(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_signingKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(_jwtLifetimeInDays);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}