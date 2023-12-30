using Taski.Api.Dtos;
using Taski.Api.Entities;
using Taski.Api.Extensions;
using Taski.Api.Repositiories;

namespace Taski.Api.Endpoints
{
    public static class ProjectEndpoints
    {
        public static void MapProjectsEndpoint(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/projects").WithParameterValidation().RequireAuthorization();

            group.MapGet("", async (IRepository<Project> projectRepository) =>
            {
                var projects = (await projectRepository.GetAllAsync()).Select(project => project.AsDto());
                return Results.Ok(projects);
            });

            group.MapGet("/user/{userId}", async (Guid userId, IRepository<Project> projectRepository) =>
            {
                var projects = (await projectRepository.GetAllAsync(project => project.UserId == userId)).Select(project => project.AsDto());
                return Results.Ok(new { projects });
            });

            group.MapGet("/{id}", async (Guid id, IRepository<Project> projectRepository) =>
            {
                var project = await projectRepository.GetAsync(id);
                if (project is null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(project.AsDto());
            });

            group.MapPost("", async (CreateProjectDto createProjectDto, IRepository<Project> projectRepository) =>
            {
                var project = new Project
                {
                    Id = Guid.NewGuid(),
                    Name = createProjectDto.Name,
                    Description = createProjectDto.Description,
                    CreateDate = DateTimeOffset.UtcNow,
                    UserId = createProjectDto.UserId
                };
                await projectRepository.CreateAsync(project);

                return Results.Created($"/api/project/{project.Id}", project.AsDto());
            });

            group.MapPut("/{id}", async (Guid id, UpdateProjectDto updateProjectDto, IRepository<Project> projectRepository) =>
            {
                var existingProject = await projectRepository.GetAsync(id);
                if (existingProject is null)
                {
                    return Results.NotFound();
                }
                existingProject.Name = updateProjectDto.Name;
                existingProject.Description = updateProjectDto.Description;
                await projectRepository.UpdateAsync(existingProject);
                return Results.NoContent();
            });

            group.MapDelete("/{id}", async (Guid id, IRepository<Project> projectRepository, IRepository<Story> storyRepository) =>
            {
                var existingProject = await projectRepository.GetAsync(id);
                if (existingProject is null)
                {
                    return Results.NotFound();
                }
                var stories = await storyRepository.GetAllAsync();

                foreach (var story in stories)
                {
                    await storyRepository.RemoveAsync(story.Id);
                }

                await projectRepository.RemoveAsync(existingProject.Id);
                return Results.NoContent();
            });
        }
    }
}