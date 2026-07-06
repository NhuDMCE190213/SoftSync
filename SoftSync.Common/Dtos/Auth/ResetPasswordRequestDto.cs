using System.ComponentModel.DataAnnotations;

namespace SoftSync.Common.Dtos;

public class ResetPasswordRequestDto
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
    [Compare(nameof(NewPassword), ErrorMessage = "Mật khẩu xác nhận không khớp")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
