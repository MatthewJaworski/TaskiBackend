using System;
namespace Taski.Api.Dtos;

public class RegisterResponseDto
{
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }
    public Dictionary<string, List<string>> Errors { get; set; } = new Dictionary<string, List<string>>();
}
