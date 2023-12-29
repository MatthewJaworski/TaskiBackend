using System;
namespace Taski.Api.Dtos;

public class LoginResponseDto
{
    public bool Success { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, List<string>> Errors { get; set; } = new Dictionary<string, List<string>>();
}
