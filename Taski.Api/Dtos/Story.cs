using System.ComponentModel.DataAnnotations;
using Taski.Api.Enums;

namespace Taski.Api.Dtos;

public record StoryDto(
    Guid Id,
    Guid ProjectId,
    Guid CreatedBy,
    Guid AssignedTo,
    string Name,
    string Description,
    DateTimeOffset CreateDate,
    DateTimeOffset? CompleteDate,
    bool IsComplete,
    int StoryPoints,
    StoryPriority Priority
);

public record CreateStoryDto(
    [Required] Guid ProjectId,
    [Required] Guid CreatedBy,
    [Required] string Name,
    Guid? AssignedTo = null,
    string Description = null,
    int? StoryPoints = null,
    StoryPriority? Priority = null
);

public record UpdateStoryDto(
    string Name,
    string Description,
    Guid? AssignedTo = null,
    int? StoryPoints = null,
    StoryPriority? Priority = null,
    bool? IsComplete = null,
    DateTimeOffset? CompleteDate = null
);
