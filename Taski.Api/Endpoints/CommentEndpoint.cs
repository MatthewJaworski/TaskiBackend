using Taski.Api.Dtos;
using Taski.Api.Entities;
using Taski.Api.Repositiories;

namespace Taski.Api.Endpoints;

public static class CommentEndpoints
{
  public static void MapCommentsEndpoint(this IEndpointRouteBuilder routes)
  {
    var group = routes.MapGroup("/api/comments").WithTags("Comments").WithParameterValidation().RequireAuthorization();

    group.MapPost("/story", async (AddCommentDto addCommentDto, IRepository<Story> storyRepository, IRepository<Comment> commentRepository) =>
    {
      var story = await storyRepository.GetAsync(s => s.Id == addCommentDto.TypeId);
      if (story == null)
      {
        return Results.NotFound(new { success = false, message = "Story not found" });
      }
      var comment = new Comment
      {
        Id = Guid.NewGuid(),
        UserId = addCommentDto.UserId,
        StoryId = addCommentDto.TypeId,
        Content = addCommentDto.Content,
        CreateDate = DateTimeOffset.UtcNow
      };
      await commentRepository.CreateAsync(comment);
      return Results.Ok(new { success = true });
    });
  }
}