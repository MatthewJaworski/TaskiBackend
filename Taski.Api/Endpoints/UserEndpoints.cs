using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Taski.Authenticator.Dtos;
using Taski.Api.Entities;
using Taski.Api.Constants;
using Taski.Api.Dtos;
namespace Taski.Api.Endpoints;

public static class UserEndpoints
{
  public static RouteGroupBuilder MapUsersEndpoint(this IEndpointRouteBuilder routes)
  {
    var group = routes.MapGroup("/api/auth").WithParameterValidation();

    group.MapPost("roles/add", async (CreateRoleDto request, RoleManager<Role> roleManager) =>
    {
      var appRole = new Role { Name = "User" };
      var createRole = await roleManager.CreateAsync(appRole);

      if (!createRole.Succeeded)
      {
        Console.WriteLine("Role creation failed with the following errors:");
        foreach (var error in createRole.Errors)
        {
          Console.WriteLine($"Code: {error.Code}, Description: {error.Description}");
        }
      }

      return Results.Ok(new { message = "Role created successfully", data = createRole });
    });

    group.MapPost("register", async (RegisterRequestDto request, IPasswordHasher<User> passwordHasher, UserManager<User> userManager) =>
    {
      var result = await RegisterAsync(request, userManager);
      if (result.Success)
      {
        var loginResult = await LoginAsync(new LoginRequestDto { Email = request.Email, Password = request.Password }, userManager, passwordHasher);
        return Results.Ok(result);
      }
      return Results.BadRequest(new
      {
        result.Success,
        result.Message,
        result.Errors
      });
    });

    group.MapPost("login", async (LoginRequestDto request, IPasswordHasher<User> passwordHasher, UserManager<User> userManager) =>
    {
      var result = await LoginAsync(request, userManager, passwordHasher);
      return result.Success ? Results.Ok(result) : Results.BadRequest(new
      {
        result.Success,
        result.Message,
        result.Errors
      });
    });

    return group;
  }

  private static async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request, UserManager<User> userManager)
  {
    try
    {
      var userExists = await userManager.FindByEmailAsync(request.Email);
      if (userExists != null)
      {
        return new RegisterResponseDto
        {
          Message = Messages.UserCreationFailed,
          Success = false,
          Errors = new Dictionary<string, List<string>>
                        {
                            { "Email", new List<string> { "Email already in use." } }
                        }
        };
      }
      userExists = new User
      {
        FullName = request.FullName,
        Email = request.Email,
        ConcurrencyStamp = Guid.NewGuid().ToString(),
        UserName = request.Email,
      };
      var createUserResult = await userManager.CreateAsync(userExists, request.Password);
      var addUserToRoleResult = await userManager.AddToRoleAsync(userExists, "User");
      return new RegisterResponseDto { Message = Messages.UserCreatedSuccessfully, Success = true };
    }

    catch (Exception ex)
    {
      return new RegisterResponseDto
      {
        Message = $"{Errors.ErrorOccured} {ex.Message}",
        Success = false
      };
    }
  }

  private static async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, UserManager<User> userManager, IPasswordHasher<User> passwordHasher)
  {
    try
    {
      var user = await userManager.FindByEmailAsync(request.Email);
      if (user == null) return new LoginResponseDto
      {
        Message = "Invalid email/password",
        Success = false,
        Errors = new Dictionary<string, List<string>>
                    {
                        { "General", new List<string> { "Invalid email/password" } }
                    }
      };

      var hashedCheckResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

      if (hashedCheckResult == PasswordVerificationResult.Failed) return new LoginResponseDto
      {
        Message = "Invalid email/password",
        Success = false,
        Errors = new Dictionary<string, List<string>>
                    {
                        { "General", new List<string> { "Invalid email/password" } }
                    }
      };
      var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim("username", user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("id", user.Id.ToString())
                };
      var roles = await userManager.GetRolesAsync(user);
      var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));
      claims.AddRange(roleClaims);

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("P5BsNuJR8hgfAx7ap9ZkW3jmGnC6rMDe"));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
      var expires = DateTime.Now.AddMinutes(120);

      var token = new JwtSecurityToken(
          issuer: "https://localhost:5001",
          audience: "https://localhost:5001",
          claims: claims,
          expires: expires,
          signingCredentials: creds

      );
      return new LoginResponseDto
      {
        AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
        Message = "Login successful",
        Email = user?.Email,
        Success = true,
        UserId = user?.Id.ToString(),
      };

    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
      return new LoginResponseDto { Success = false, Message = ex.Message };
    }
  }
}