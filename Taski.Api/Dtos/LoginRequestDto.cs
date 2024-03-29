using System.ComponentModel.DataAnnotations;

namespace Taski.Api.Dtos;

public class LoginRequestDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    public bool Success { get; set; }

}
