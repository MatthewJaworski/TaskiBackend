namespace Taski.Api.Entities;

public class Comment
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public User User { get; set; }
  public Guid StoryId { get; set; }
  public Story Story { get; set; }
  public string Content { get; set; }
  public DateTimeOffset CreateDate { get; set; }
}