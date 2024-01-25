using Taski.Api.Dtos;

namespace Taski.Api.Extensions;
public static class StoryTagExtensions
{
    public static TagDto AsDto(this StoryTag projectTag)
    {
        if (projectTag == null)
        {
            return null;
        }

        return new TagDto
        {
            Id = projectTag.Id,
            Name = projectTag.Name,
        };
    }
}