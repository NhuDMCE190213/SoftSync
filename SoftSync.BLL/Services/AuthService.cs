using SoftSync.BLL.Interfaces;
using SoftSync.Common.Dtos;
using SoftSync.Common.Security;
using SoftSync.DAL.Entities;
using SoftSync.DAL.Repositories;

namespace SoftSync.BLL.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly IPasswordResetTokenRepository _resetTokenRepo;
    private readonly IEmailService _emailService;

    public AuthService(IUserRepository userRepo, IPasswordResetTokenRepository resetTokenRepo, IEmailService emailService)
    {
        _userRepo = userRepo;
        _resetTokenRepo = resetTokenRepo;
        _emailService = emailService;
    }

    public async Task<AuthResultDto> RegisterAsync(RegisterRequestDto dto)
    {
        if (dto.Password != dto.ConfirmPassword)
            return AuthResultDto.Fail("Mật khẩu xác nhận không khớp.");

        if (await _userRepo.EmailExistsAsync(dto.Email))
            return AuthResultDto.Fail("Email này đã được đăng ký.");

        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email.Trim().ToLower(),
            PasswordHash = PasswordHasher.Hash(dto.Password),
            Age = dto.Age,
            Goal = dto.Goal,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepo.AddAsync(user);
        await _userRepo.SaveChangesAsync();

        var userDto = new UserDto { Id = user.Id, FullName = user.FullName, Email = user.Email, Age = user.Age, Role = user.Role, Goal = user.Goal, CreatedAt = user.CreatedAt };
        return AuthResultDto.Ok("Đăng ký thành công!", userDto);
    }

    public async Task<AuthResultDto> LoginAsync(LoginRequestDto dto)
    {
        var user = await _userRepo.GetByEmailAsync(dto.Email.Trim());
        if (user == null || !PasswordHasher.Verify(dto.Password, user.PasswordHash))
            return AuthResultDto.Fail("Email hoặc mật khẩu không đúng.");

        var userDto = new UserDto { Id = user.Id, FullName = user.FullName, Email = user.Email, Age = user.Age, Role = user.Role, Goal = user.Goal, CreatedAt = user.CreatedAt };
        return AuthResultDto.Ok("Đăng nhập thành công!", userDto);
    }

    public async Task<AuthResultDto> ForgotPasswordAsync(ForgotPasswordRequestDto dto, string resetLinkBaseUrl)
    {
        // Luôn trả về thông báo chung chung để tránh lộ email nào tồn tại trong hệ thống
        const string genericMessage = "Nếu email tồn tại trong hệ thống, chúng tôi đã gửi hướng dẫn đặt lại mật khẩu.";

        var user = await _userRepo.GetByEmailAsync(dto.Email.Trim());
        if (user == null)
            return AuthResultDto.Ok(genericMessage);

        var token = GenerateSecureToken();
        var resetToken = new PasswordResetToken
        {
            UserId = user.Id,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };
        await _resetTokenRepo.AddAsync(resetToken);
        await _resetTokenRepo.SaveChangesAsync();

        var resetLink = $"{resetLinkBaseUrl.TrimEnd('/')}/reset-password?token={Uri.EscapeDataString(token)}";

        try
        {
            await _emailService.SendPasswordResetEmailAsync(user.Email, user.FullName, resetLink);
        }
        catch
        {
            // Không lộ lỗi SMTP cho người dùng để tránh dò thông tin hệ thống,
            // nhưng vẫn có thể log lại ở tầng SmtpEmailService.
        }

        return AuthResultDto.Ok(genericMessage);
    }

    public async Task<AuthResultDto> ResetPasswordAsync(ResetPasswordRequestDto dto)
    {
        if (dto.NewPassword != dto.ConfirmPassword)
            return AuthResultDto.Fail("Mật khẩu xác nhận không khớp.");

        var resetToken = await _resetTokenRepo.GetValidTokenAsync(dto.Token);
        if (resetToken == null)
            return AuthResultDto.Fail("Liên kết đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.");

        resetToken.User.PasswordHash = PasswordHasher.Hash(dto.NewPassword);
        resetToken.IsUsed = true;

        _resetTokenRepo.Update(resetToken);
        await _resetTokenRepo.SaveChangesAsync();

        return AuthResultDto.Ok("Đặt lại mật khẩu thành công! Bạn có thể đăng nhập ngay bây giờ.");
    }

    private static string GenerateSecureToken()
    {
        var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

    public async Task<AuthResultDto> ChangePasswordAsync(int userId, string currentPassword, string newPassword, string confirmPassword)
    {
        if (newPassword != confirmPassword)
            return AuthResultDto.Fail("Mật khẩu xác nhận không khớp.");

        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null)
            return AuthResultDto.Fail("Không tìm thấy tài khoản.");

        if (!PasswordHasher.Verify(currentPassword, user.PasswordHash))
            return AuthResultDto.Fail("Mật khẩu hiện tại không đúng.");

        user.PasswordHash = PasswordHasher.Hash(newPassword);
        _userRepo.Update(user);
        await _userRepo.SaveChangesAsync();

        return AuthResultDto.Ok("Đổi mật khẩu thành công!");
    }
}