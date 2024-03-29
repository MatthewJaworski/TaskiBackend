namespace Taski.Api.Entities;

public class Project
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public User User { get; set; }
  public string Name { get; set; }
  public string Description { get; set; }
  public DateTimeOffset CreateDate { get; set; }
  public ICollection<Story> Stories { get; set; } = new List<Story>();
  public ICollection<ProjectTagAssociation> TagAssociations { get; set; } = new List<ProjectTagAssociation>();
  public ICollection<UserProjectAssociation> UserProjectAssociations { get; set; } = new List<UserProjectAssociation>();
}