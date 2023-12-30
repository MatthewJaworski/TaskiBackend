// Story.cs
using Taski.Api.Enums;

namespace Taski.Api.Entities;

public class Story
{
  public Guid Id { get; set; }
  public Guid ProjectId { get; set; }
  public Project Project { get; set; }
  public Guid CreatedBy { get; set; }
  public User CreatedByUser { get; set; }
  public Guid AssignedTo { get; set; }
  public User AssignedToUser { get; set; }
  public string Name { get; set; }
  public string Description { get; set; }
  public DateTimeOffset CreateDate { get; set; }
  public DateTimeOffset? CompleteDate { get; set; }
  public bool IsComplete { get; set; }
  public int StoryPoints { get; set; }
  public StoryPriority Priority { get; set; }
  public StoryTag Tag { get; set; }

  public Guid TagId { get; set; }
}