using System;
using System.ComponentModel.DataAnnotations;
using Taski.Api.Constants;
namespace Taski.Authenticator.Dtos;

public class RegisterRequestDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Username can only contain alphanumeric characters.")]
    public string Username { get; set; } = string.Empty;
    [Required]
    [Display(Name = "Full Name")]
    [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Full Name can only contain letters and spaces.")]
    public string FullName { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).*$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
    public string Password { get; set; } = string.Empty;
    [Required, DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = Errors.PassowrdNotMatch)]
    public string ConfirmPassword { get; set; } = string.Empty;
}
