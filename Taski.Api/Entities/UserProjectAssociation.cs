namespace Taski.Api.Entities;

public class UserProjectAssociation
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public User User { get; set; }
  public Guid ProjectId { get; set; }
  public Project Project { get; set; }
}