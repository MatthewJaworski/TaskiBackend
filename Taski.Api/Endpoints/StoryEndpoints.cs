using Taski.Api.Dtos;
using Taski.Api.Entities;
using Taski.Api.Enums;
using Taski.Api.Extensions;
using Taski.Api.Repositiories;

namespace Taski.Api.Endpoints
{
    public static class StoryEndpoints
    {
        public static void MapStoriesEndpoint(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/stories").WithParameterValidation().RequireAuthorization();

            group.MapGet("", async (IRepository<Story> storyRepository) =>
            {
                var stories = (await storyRepository.GetAllAsync()).Select(story => story.AsDto());
                return Results.Ok(stories);
            });

            group.MapGet("/project/{projectId}", async (Guid projectId, IRepository<Story> storyRepository) =>
            {
                var stories = (await storyRepository.GetAllAsync()).Where(story => story.ProjectId == projectId).Select(story => story.AsDto());
                return Results.Ok(stories);
            });

            group.MapGet("/{id}", async (Guid id, IRepository<Story> storyRepository) =>
            {
                var story = await storyRepository.GetAsync(id);
                if (story is null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(story.AsDto());
            });

            group.MapPost("", async (CreateStoryDto createStoryDto, IRepository<Story> storyRepository) =>
            {
                var story = new Story
                {
                    Id = Guid.NewGuid(),
                    ProjectId = createStoryDto.ProjectId,
                    CreatedBy = createStoryDto.CreatedBy,
                    Name = createStoryDto.Name,
                    Description = createStoryDto.Description,
                    AssignedTo = createStoryDto.AssignedTo ?? Guid.Empty,
                    CompleteDate = null,
                    CreateDate = DateTimeOffset.UtcNow,
                    IsComplete = false,
                    Priority = createStoryDto.Priority ?? StoryPriority.Low,
                    StoryPoints = createStoryDto.StoryPoints ?? 0
                };
                await storyRepository.CreateAsync(story);
                return Results.Created($"/api/stories/{story.Id}", story.AsDto());
            });

            group.MapPut("/{id}", async (Guid id, UpdateStoryDto updateStoryDto, IRepository<Story> storyRepository) =>
            {
                var existingStory = await storyRepository.GetAsync(id);
                if (existingStory is null)
                {
                    return Results.NotFound();
                }
                existingStory.Name = updateStoryDto.Name;
                existingStory.Description = updateStoryDto.Description;
                if (updateStoryDto.AssignedTo.HasValue)
                {
                    existingStory.AssignedTo = updateStoryDto.AssignedTo.Value;
                }
                if (updateStoryDto.StoryPoints.HasValue)
                {
                    existingStory.StoryPoints = updateStoryDto.StoryPoints.Value;
                }
                if (updateStoryDto.Priority.HasValue)
                {
                    existingStory.Priority = updateStoryDto.Priority.Value;
                }
                if (updateStoryDto.IsComplete.HasValue)
                {
                    existingStory.IsComplete = updateStoryDto.IsComplete.Value;
                }
                if (updateStoryDto.CompleteDate.HasValue)
                {
                    existingStory.CompleteDate = updateStoryDto.CompleteDate.Value;
                }
                await storyRepository.UpdateAsync(existingStory);
                return Results.NoContent();
            });

            group.MapDelete("/{id}", async (Guid id, IRepository<Story> storyRepository) =>
            {
                var existingStory = await storyRepository.GetAsync(id);
                if (existingStory is null)
                {
                    return Results.NotFound();
                }
                await storyRepository.RemoveAsync(existingStory.Id);
                return Results.NoContent();
            });
        }
    }
}