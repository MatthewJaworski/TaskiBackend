namespace Taski.Api.Dtos;
public class CommentDto
{
  public Guid Id { get; set; }
  public string FullName { get; set; }
  public string Content { get; set; }
  public DateTimeOffset CreateDate { get; set; }

}