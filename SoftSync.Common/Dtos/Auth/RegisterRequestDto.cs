using System.ComponentModel.DataAnnotations;

namespace SoftSync.Common.Dtos;

public class RegisterRequestDto
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
    [Compare(nameof(Password), ErrorMessage = "Mật khẩu xác nhận không khớp")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Range(1, 120)]
    public int Age { get; set; }

    [MaxLength(500)]
    public string Goal { get; set; } = string.Empty;
}
