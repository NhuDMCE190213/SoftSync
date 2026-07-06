using System.ComponentModel.DataAnnotations;

namespace SoftSync.DAL.Entities;

// MỚI: token dùng cho tính năng quên mật khẩu
public class PasswordResetToken
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    [Required, MaxLength(200)]
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
