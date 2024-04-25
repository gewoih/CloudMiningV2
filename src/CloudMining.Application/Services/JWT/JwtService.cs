using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CloudMining.Domain.Models.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CloudMining.Application.Services.JWT;

public class JwtService
{
    private readonly string _signingKey;
    private readonly int _jwtLifetimeInDays;

    public JwtService(IConfiguration configuration)
    {
        _signingKey = configuration.GetValue<string>("Jwt:SigningKey");
        _jwtLifetimeInDays = configuration.GetValue<int>("Jwt:LifetimeInDays");
    }

    public string Generate(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString())
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