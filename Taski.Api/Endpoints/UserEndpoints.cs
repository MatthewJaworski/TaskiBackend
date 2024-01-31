using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Taski.Authenticator.Dtos;
using Taski.Api.Entities;
using Taski.Api.Constants;
using Taski.Api.Dtos;
using Taski.Api.Repositiories;
using Taski.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Taski.Api.Utils;
using Microsoft.EntityFrameworkCore;
namespace Taski.Api.Endpoints;

public static class UserEndpoints
{
  public static RouteGroupBuilder MapUsersEndpoint(this IEndpointRouteBuilder routes)
  {
    var group = routes.MapGroup("/api/user").WithTags("User").WithParameterValidation();

    group.MapPost("roles/add", async (CreateRoleDto request, RoleManager<Role> roleManager) =>
    {
      var appRole = new Role { Name = request.Role };
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


    group.MapGet("", async (IRepository<User> userRepository) =>
    {
      var users = (await userRepository.GetAllAsync()).Select(user => user.AsDto());
      return Results.Ok(users);
    }).RequireAuthorization();


    group.MapPost("/refresh-token", async (RefreshTokenRequestDto request, IRepository<User> userRepository, UserManager<User> userManager) =>
    {
      var handler = new JwtSecurityTokenHandler();
      JwtSecurityToken jwtToken;
      try
      {
        jwtToken = handler.ReadJwtToken(request.token);
      }
      catch (Exception ex)
      {
        return Results.Unauthorized();
      }
      var id = jwtToken.Claims.First(claim => claim.Type == "id").Value;
      var user = (await userRepository.GetAllAsync(u => u.Id == Guid.Parse(id))).SingleOrDefault();
      if (user == null)
      {
        return Results.Unauthorized();
      }
      var tokenGenerator = new JwtTokenGenerator(userManager, "P5BsNuJR8hgfAx7ap9ZkW3jmGnC6rMDe");
      var token = await tokenGenerator.GenerateToken(user);
      var reponse = new JwtSecurityTokenHandler().WriteToken(token);
      return Results.Ok(new { refreshedToken = reponse });
    });

    group.MapGet("/{id}", async (IRepository<User> userRepository, Guid id) =>
    {
      var user = await userRepository.GetAll()
        .Where(u => u.Id == id)
        .Include(u => u.Projects)
        .Include(u => u.AssignedStories)
        .Include(u => u.CreatedStories)
        .Include(u => u.UserProjectAssociations)
        .SingleOrDefaultAsync();

      if (user == null)
      {
        return Results.NotFound(new { success = false, message = "User not found" });
      }
      return Results.Ok(user.AsWholeDto());
    }).RequireAuthorization("Admin");

    group.MapDelete("/{id}", async (IRepository<User> userRepository, IRepository<Project> projectRepository, Guid id) =>
    {
      var user = await userRepository.GetAsync(u => u.Id == id);
      if (user == null)
      {
        return Results.NotFound(new { success = false, message = "User not found" });
      }
      var userProjects = await projectRepository.GetAllAsync(p => p.UserId == id);

      foreach (var project in userProjects)
      {
        await projectRepository.RemoveAsync(project.Id);
      }
      await userRepository.RemoveAsync(user.Id);
      return Results.Ok(new { success = true, message = "User deleted successfully" });
    }).RequireAuthorization();

    group.MapPost("/project", async (AddUserToProjectDto addUserToProjectDto, IRepository<User> userRepository, IRepository<Project> projectRepository, IRepository<UserProjectAssociation> userProjectAssociation) =>
    {
      var user = await userRepository.GetAsync(u => u.Id == addUserToProjectDto.userId);
      if (user == null)
      {
        return Results.NotFound(new { success = false, message = "User not found" });
      }
      var project = await projectRepository.GetAsync(p => p.Id == addUserToProjectDto.projectId);
      if (project == null)
      {
        return Results.NotFound(new { success = false, message = "Project not found" });
      }
      var existingAssociation = await userProjectAssociation.GetAsync(upa => upa.UserId == user.Id && upa.ProjectId == project.Id);
      if (existingAssociation != null)
      {
        return Results.Conflict(new { success = false, message = "User is already in the project" });
      }
      var userProject = new UserProjectAssociation
      {
        UserId = user.Id,
        ProjectId = project.Id
      };
      await userProjectAssociation.CreateAsync(userProject);
      return Results.Ok(new { success = true, message = "User added to project successfully" });
    }).RequireAuthorization();

    group.MapDelete("/project", async ([FromBody] AddUserToProjectDto addUserToProjectDto, IRepository<User> userRepository, IRepository<Project> projectRepository, IRepository<UserProjectAssociation> userProjectAssociation) =>
    {
      var user = await userRepository.GetAsync(u => u.Id == addUserToProjectDto.userId);
      if (user == null)
      {
        return Results.NotFound(new { success = false, message = "User not found" });
      }
      var project = await projectRepository.GetAsync(p => p.Id == addUserToProjectDto.projectId);
      if (project == null)
      {
        return Results.NotFound(new { success = false, message = "Project not found" });
      }
      var existingAssociation = await userProjectAssociation.GetAsync(upa => upa.UserId == user.Id && upa.ProjectId == project.Id);
      if (existingAssociation == null)
      {
        return Results.Conflict(new { success = false, message = "User is not in the project" });
      }
      await userProjectAssociation.RemoveAsync(existingAssociation.Id);
      return Results.Ok(new { success = true, message = "User removed from project successfully" });
    }).RequireAuthorization();
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
        UserName = request.Username,
      };
      var createUserResult = await userManager.CreateAsync(userExists, request.Password);
      if (!createUserResult.Succeeded)
      {
        return new RegisterResponseDto
        {
          Message = "User creation failed: " + string.Join(", ", createUserResult.Errors.Select(x => x.Description)),
          Success = false
        };
      }

      var addUserToRoleResult = await userManager.AddToRoleAsync(userExists, request.Role);

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

      var tokenGenerator = new JwtTokenGenerator(userManager, "P5BsNuJR8hgfAx7ap9ZkW3jmGnC6rMDe");
      var token = await tokenGenerator.GenerateToken(user);

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