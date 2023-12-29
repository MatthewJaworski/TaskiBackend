using Taski.Api.Dtos;
using Taski.Api.Entities;

namespace Taski.Api.Extensions;

public static class StoryExtensions
{
  public static StoryDto AsDto(this Story story)
  {
    return new StoryDto(
        story.Id,
        story.ProjectId,
        story.CreatedBy,
        story.AssignedTo,
        story.Name,
        story.Description,
        story.CreateDate,
        story.CompleteDate,
        story.IsComplete,
        story.StoryPoints,
        story.Priority
    );
  }
}
