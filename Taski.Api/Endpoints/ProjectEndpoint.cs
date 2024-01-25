using Microsoft.EntityFrameworkCore;
using Taski.Api.Dtos;
using Taski.Api.Entities;
using Taski.Api.Extensions;
using Taski.Api.Repositiories;

namespace Taski.Api.Endpoints;

public static class ProjectEndpoints
{
    public static void MapProjectsEndpoint(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/projects").WithTags("Project").WithParameterValidation().RequireAuthorization();

        group.MapGet("", async (IRepository<Project> projectRepository) =>
        {
            var projects = (await projectRepository.GetAllAsync()).Select(project => project.AsDto());
            return Results.Ok(new {projects});
        });

        group.MapGet("/user/{userId}", async (Guid userId, IRepository<Project> projectRepository, IRepository<UserProjectAssociation> userProjectAssociationRepository) =>
        {
            var projectIds = (await userProjectAssociationRepository.GetAllAsync(upa => upa.UserId == userId)).Select(upa => upa.ProjectId);

            var projects = await projectRepository.GetAllAsync(
                project => project.TagAssociations,
                project => project.Stories,
                project => project.UserProjectAssociations,
                project => project.User);

            projects = projects.Where(project => projectIds.Contains(project.Id))
                .OrderByDescending(project => project.CreateDate)
                .ToList();

            return Results.Ok(new { Projects = projects.Select(project => project.AsDto()) });
        });

        group.MapGet("/{id}", async (Guid id, IRepository<Project> projectRepository) =>
        {
            var project = await projectRepository
                .GetAll()
                .Include(project => project.TagAssociations)
                .ThenInclude(ta => ta.ProjectTag)
                .Include(project => project.Stories)
                .Include(project => project.UserProjectAssociations)
                        .ThenInclude(up => up.User)
                .SingleOrDefaultAsync(project => project.Id == id);

            if (project is null)
            {
                return Results.NotFound();
            }
            return Results.Ok(project.AsDto());
        });

        group.MapPost("", async (CreateProjectDto createProjectDto, IRepository<Project> projectRepository, IRepository<ProjectTag> tagRepository, IRepository<ProjectTagAssociation> tagAssociationRepository, IRepository<UserProjectAssociation> userProjectAssociation) =>
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

            foreach (var tagName in createProjectDto.Tags)
            {
                var tag = await tagRepository.GetAsync(t => t.Name == tagName);
                if (tag == null)
                {
                    tag = new ProjectTag
                    {
                        Name = tagName
                    };
                    await tagRepository.CreateAsync(tag);
                }

                var tagAssociation = new ProjectTagAssociation
                {
                    ProjectId = project.Id,
                    ProjectTagId = tag.Id
                };
                await tagAssociationRepository.CreateAsync(tagAssociation);

            }

            var userProjectAssociationEntity = new UserProjectAssociation
            {
                UserId = project.UserId,
                ProjectId = project.Id
            };

            await userProjectAssociation.CreateAsync(userProjectAssociationEntity);

            return Results.Ok(new { success = true, project = project.AsDto() });
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

        group.MapGet("/{projectId}/users", async (Guid projectId, IRepository<UserProjectAssociation> userProjectAssociationRepository, IRepository<User> userRepository) =>
        {
            var userProjectAssociations = await userProjectAssociationRepository.GetAllAsync(upa => upa.ProjectId == projectId);

            var users = new List<UserDto>();

            foreach (var upa in userProjectAssociations)
            {
                var user = await userRepository.GetAsync(upa.UserId);
                if (user != null)
                {
                    users.Add(user.AsDto());
                }
            }
            return Results.Ok(users);
        });
    }
}
