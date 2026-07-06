using SoftSync.Common.Enums;

namespace SoftSync.Common.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Age { get; set; }
    public UserRole Role { get; set; }
    public string Goal { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
