using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Taski.Api.Entities;

namespace Taski.Api.Utils;

public class JwtTokenGenerator
{
  private readonly UserManager<User> _userManager;
  private readonly string _secretKey;

  public JwtTokenGenerator(UserManager<User> userManager, string secretKey)
  {
    _userManager = userManager;
    _secretKey = secretKey;
  }

  public async Task<JwtSecurityToken> GenerateToken(User user)
  {
    var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim("username", user.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("id", user.Id.ToString()),
        new Claim("email", user.Email),
        new Claim("fullName", user.FullName),
    };

    var roles = await _userManager.GetRolesAsync(user);
    var roleClaims = roles.Select(role => new Claim("role", role));
    claims.AddRange(roleClaims);

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var expires = DateTime.Now.AddMinutes(120);

    var token = new JwtSecurityToken(
        issuer: "https://localhost:5001",
        audience: "https://localhost:5001",
        claims: claims,
        expires: expires,
        signingCredentials: creds
    );

    return token;
  }
}