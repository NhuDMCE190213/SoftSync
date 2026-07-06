namespace SoftSync.Common.Dtos.Auth;

public class AuthResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public UserDto? User { get; set; }

    public static AuthResultDto Fail(string message) => new() { Success = false, Message = message };
    public static AuthResultDto Ok(string message, UserDto? user = null) => new() { Success = true, Message = message, User = user };
}