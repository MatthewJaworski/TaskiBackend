using Taski.Api.Dtos;
using Taski.Api.Entities;

namespace Taski.Api.Extensions;

public static class StoryExtensions
{
  public static StoryDto AsDto(this Story story)
  {
    TagDto tagDto = null;
    if (story.Tag != null)
    {
      tagDto = story.Tag.AsDto();
    }

    UserDto createdByDto = story.CreatedByUser != null ? story.CreatedByUser.AsDto() : null;
    UserDto assignedToDto = story.AssignedToUser != null ? story.AssignedToUser.AsDto() : null;
    List<CommentDto> comments = story.Comments.Select(c => c.AsDto()).ToList();

    return new StoryDto(
        story.Id,
        story.ProjectId,
        createdByDto,
        assignedToDto,
        story.Name,
        story.Description,
        story.CreateDate,
        story.CompleteDate,
        story.IsComplete,
        story.StoryPoints,
        story.Priority,
        tagDto,
        comments
    );
  }
}
