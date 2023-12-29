using Taski.Api.Dtos;
using Taski.Api.Entities;

namespace Taski.Api.Extensions;
public static class ProjectExtensions
{
  public static ProjectDto AsDto(this Project project)
  {
    return new ProjectDto(project.Id, project.UserId, project.Name, project.Description, project.CreateDate);
  }
}
