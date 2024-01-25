
namespace Taski.Api.Dtos;
public class UserDto
{
  public Guid Id { get; set; }
  public string Name { get; set; }

}

public class UserWholeDto
{
  public Guid Id { get; set; }
  public string FullName { get; set; }
  public string Email { get; set; }
  public string Username { get; set; }
  public ICollection<ProjectDto> Projects { get; set; }
  public ICollection<StoryDto> AssignedStories { get; set; }
}