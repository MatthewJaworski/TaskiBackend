using Taski.Api.Dtos;
using Taski.Api.Entities;

namespace Taski.Api.Extensions;
public static class ProjectExtensions
{
  public static ProjectDto AsDto(this Project project)
  {
    return new ProjectDto(project.Id,
      project.UserId,
      project.Name,
      project.Description,
      project.CreateDate,
      project.TagAssociations.Where(ta => ta != null && ta.ProjectTag != null).Select(ta => ta.ProjectTag.Name).ToList(),
      project.Stories.Where(story => story != null).Select(story => story.AsDto()).ToList(),
      project.UserProjectAssociations.Select(up => up.User.AsDto()).ToList()
    );
  }
}
