using Taski.Api.Dtos;
using Taski.Api.Entities;

namespace Taski.Api.Extensions;
public static class UserExtensions
{
    public static UserDto AsDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.FullName
        };
    }
}