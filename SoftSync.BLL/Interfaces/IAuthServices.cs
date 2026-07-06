using SoftSync.Common.Dtos;
using SoftSync.Common.Dtos.Auth;

namespace SoftSync.BLL.Interfaces;

public interface IAuthService
{
    Task<AuthResultDto> RegisterAsync(RegisterRequestDto dto);
    Task<AuthResultDto> LoginAsync(LoginRequestDto dto);
    Task<AuthResultDto> ForgotPasswordAsync(ForgotPasswordRequestDto dto, string resetLinkBaseUrl);
    Task<AuthResultDto> ResetPasswordAsync(ResetPasswordRequestDto dto);
    Task<AuthResultDto> ChangePasswordAsync(int userId, string currentPassword, string newPassword, string confirmPassword); // MỚI
}

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string toEmail, string toName, string resetLink);
}