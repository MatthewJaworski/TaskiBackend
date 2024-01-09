using System.ComponentModel.DataAnnotations;
using Taski.Api.Enums;

namespace Taski.Api.Dtos;

public record StoryDto(
    Guid Id,
    Guid ProjectId,
    UserDto CreatedBy,
    UserDto? AssignedTo,
    string Name,
    string Description,
    DateTimeOffset CreateDate,
    DateTimeOffset? CompleteDate,
    bool IsComplete,
    int StoryPoints,
    StoryPriority Priority,
    TagDto? tag,
    List<CommentDto> Comments
);

public record CreateStoryDto(
    [Required] Guid ProjectId,
    [Required] Guid CreatedBy,
    [Required] string Name,
    Guid? AssignedTo = null,
    string Description = null,
    int? StoryPoints = null,
    StoryPriority? Priority = null,
    string Tag = null
);

public class UpdateStoryDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public UserDto CreatedBy { get; set; }
    public UserDto AssignedTo { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? CompleteDate { get; set; }
    public bool IsComplete { get; set; }
    public int StoryPoints { get; set; }
    public StoryPriority Priority { get; set; }
    public TagDto Tag { get; set; }
}