using Taski.Api.Entities;

public class ProjectTag : ITag
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ICollection<ProjectTagAssociation> ProjectAssociations { get; set; }
}