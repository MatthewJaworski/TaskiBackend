using Taski.Api.Dtos;
using Taski.Api.Entities;

namespace Taski.Api.Extensions;
public static class UserExtensions
{
    public static UserDto AsDto(this User user)
    {
        if (user == null)
        {
            return null;
        }

        return new UserDto
        {
            Id = user.Id,
            Name = user.FullName
        };
    }
    public static UserWholeDto AsWholeDto(this User user)
    {
        if (user == null)
        {
            return null;
        }

        return new UserWholeDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Username = user.UserName,
            Projects = user.Projects.Select(p => p.AsDto()).ToList(),
            AssignedStories = user.AssignedStories.Select(s => s.AsDto()).ToList()
        };
    }
}