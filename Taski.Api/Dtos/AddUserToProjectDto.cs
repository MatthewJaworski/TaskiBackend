namespace Taski.Api.Dtos;
public class AddUserToProjectDto
{
  public Guid projectId { get; set; }
  public Guid userId { get; set; }

}