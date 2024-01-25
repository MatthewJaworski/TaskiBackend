using Microsoft.EntityFrameworkCore;
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
            var group = routes.MapGroup("/api/stories").WithTags("Story").WithParameterValidation().RequireAuthorization();

            group.MapGet("", async (IRepository<Story> storyRepository) =>
            {
                var stories = (await storyRepository.GetAllAsync()).Select(story => story.AsDto());
                return Results.Ok(stories);
            });

            group.MapGet("/project/{projectId}", async (Guid projectId, IRepository<Story> storyRepository) =>
            {
                var stories = (await storyRepository.GetAllAsync())
                .Where(story => story.ProjectId == projectId).Select(story => story.AsDto());
                return Results.Ok(stories);
            });

            group.MapGet("/{id}", async (Guid id, IRepository<Story> storyRepository) =>
            {
                var story = await storyRepository.GetAll()
                    .Include(s => s.Tag)
                    .Include(s => s.CreatedByUser)
                    .Include(s => s.AssignedToUser)
                    .Include(s => s.Comments)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (story is null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(story.AsDto());
            });

            group.MapPost("", async (CreateStoryDto createStoryDto, IRepository<Story> storyRepository, IRepository<StoryTag> storyTagRepository) =>
            {
                var tag = (await storyTagRepository.GetAllAsync()).FirstOrDefault(t => t.Name == createStoryDto.Tag);

                if (tag == null && !string.IsNullOrEmpty(createStoryDto.Tag))
                {
                    tag = new StoryTag { Name = createStoryDto.Tag };
                    await storyTagRepository.CreateAsync(tag);
                }

                var story = new Story
                {
                    Id = Guid.NewGuid(),
                    ProjectId = createStoryDto.ProjectId,
                    CreatedBy = createStoryDto.CreatedBy,
                    Name = createStoryDto.Name,
                    Description = createStoryDto.Description,
                    AssignedTo = createStoryDto.AssignedTo ?? null,
                    CompleteDate = null,
                    CreateDate = DateTimeOffset.UtcNow,
                    IsComplete = false,
                    Priority = createStoryDto.Priority ?? StoryPriority.Low,
                    StoryPoints = createStoryDto.StoryPoints ?? 0,

                };
                if (tag != null)
                {
                    story.TagId = tag.Id;
                }
                await storyRepository.CreateAsync(story);
                return Results.Created($"/api/stories/{story.Id}", new
                {
                    story = story.AsDto(),
                    success = true
                });
            });

            group.MapPut("/{id}", async (Guid id, UpdateStoryDto updateStoryDto, IRepository<Story> storyRepository) =>
            {
                try
                {
                    var existingStory = await storyRepository.GetAsync(id);
                    if (existingStory is null)
                    {
                        return Results.NotFound();
                    }
                    existingStory.Name = updateStoryDto.Name;
                    if (updateStoryDto.CreatedBy != null && updateStoryDto.CreatedBy.Id != Guid.Empty)
                    {
                        existingStory.CreatedBy = updateStoryDto.CreatedBy.Id;
                    }
                    if (updateStoryDto.AssignedTo != null && updateStoryDto.AssignedTo.Id != Guid.Empty)
                    {
                        existingStory.AssignedTo = updateStoryDto.AssignedTo.Id;
                    }

                    existingStory.StoryPoints = updateStoryDto.StoryPoints;

                    existingStory.Priority = updateStoryDto.Priority;

                    existingStory.IsComplete = updateStoryDto.IsComplete;

                    if (updateStoryDto.CompleteDate.HasValue)
                    {
                        existingStory.CompleteDate = updateStoryDto.CompleteDate.Value;
                    }
                    await storyRepository.UpdateAsync(existingStory);
                    return Results.Ok(new { success = true });
                }
                catch (Exception ex)
                {
                    return Results.Problem();
                }

            });

            group.MapDelete("/{id}", async (Guid id, IRepository<Story> storyRepository) =>
            {
                var existingStory = await storyRepository.GetAsync(id);
                if (existingStory is null)
                {
                    return Results.NotFound();
                }
                await storyRepository.RemoveAsync(existingStory.Id);
                return Results.Ok(new { success = true });
            });

            group.MapGet("/user/{id}", async (Guid id, IRepository<Story> storyRepository) =>
            {
                var stories = (await storyRepository.GetAllAsync())
                .Where(story => story.AssignedTo == id).Select(story => story.AsDto()).OrderByDescending(story => story.CreateDate).ToList();
                return Results.Ok(stories);
            });
        }
    }
}