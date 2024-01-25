using Taski.Api.Dtos;

namespace Taski.Api.Extensions;
public static class ProjectTagExtensions
{
    public static TagDto AsDto(this ProjectTag projectTag)
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