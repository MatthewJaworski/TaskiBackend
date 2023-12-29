using Microsoft.AspNetCore.Identity;

namespace Taski.Api.Entities;

public class User : IdentityUser<Guid>
{
  public string FullName { get; set; } = string.Empty;
  public ICollection<Project> Projects { get; set; } = new List<Project>();
  public ICollection<Story> AssignedStories { get; set; } = new List<Story>();
  public ICollection<Story> CreatedStories { get; set; } = new List<Story>();
}
