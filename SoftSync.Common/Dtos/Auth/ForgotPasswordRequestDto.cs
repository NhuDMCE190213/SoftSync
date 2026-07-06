using System.ComponentModel.DataAnnotations;

namespace SoftSync.Common.Dtos.Auth;

public class ForgotPasswordRequestDto
{
    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = string.Empty;
}
