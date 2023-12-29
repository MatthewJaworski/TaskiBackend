using System.ComponentModel.DataAnnotations;

namespace Taski.Api.Dtos;

public record ProjectDto(Guid Id,
    Guid UserId,
    string Name,
    string Description,
    DateTimeOffset CreateDate
);
public record CreateProjectDto(
    [Required] Guid UserId,
    [Required] string Name,
    string Description
);
public record UpdateProjectDto(
    string Name,
    string Description
);


