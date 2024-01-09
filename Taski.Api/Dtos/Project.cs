using System.ComponentModel.DataAnnotations;

namespace Taski.Api.Dtos;

public record ProjectDto(Guid Id,
    Guid UserId,
    string Name,
    string Description,
    DateTimeOffset CreateDate,
    List<string> Tags,
    List<StoryDto> Stories,
    List<UserDto> Users

);
public record CreateProjectDto(
    [Required] Guid UserId,
    [Required] string Name,
    string Description,
    List<string> Tags
);
public record UpdateProjectDto(
    string Name,
    string Description
);


