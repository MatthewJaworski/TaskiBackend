namespace Taski.Api.Dtos
{
  public class AddCommentDto
  {
    public Guid TypeId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; }

  }
}