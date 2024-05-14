using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CloudMining.Domain.Models.Identity;
using CloudMining.Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CloudMining.Application.Services.JWT;

public class JwtService
{
    private readonly string _signingKey;
    private readonly int _jwtLifetimeInDays;
    private readonly UserManager<User> _userManager;

    public JwtService(IOptions<JwtSettings> settings, UserManager<User> userManager)
    {
        _userManager = userManager;
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
    
    public async Task<string> GenerateAsync(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        };

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        
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