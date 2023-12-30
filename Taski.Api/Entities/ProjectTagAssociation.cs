
namespace Taski.Api.Entities;

public class ProjectTagAssociation
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; }

    public Guid ProjectTagId { get; set; }
    public ProjectTag ProjectTag { get; set; }
}