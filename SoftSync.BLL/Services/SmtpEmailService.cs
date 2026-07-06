using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SoftSync.BLL.Interfaces;

namespace SoftSync.BLL.Services;

/// <summary>
/// Gửi email đặt lại mật khẩu qua SMTP thật.
/// Cấu hình nằm ở appsettings.json, mục "Smtp".
/// </summary>
public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string toName, string resetLink)
    {
        var host = _configuration["Smtp:Host"];
        var port = int.Parse(_configuration["Smtp:Port"] ?? "587");
        var username = _configuration["Smtp:Username"];
        var password = _configuration["Smtp:Password"];
        var fromEmail = _configuration["Smtp:FromEmail"] ?? username;
        var fromName = _configuration["Smtp:FromName"] ?? "SoftSync";
        var enableSsl = bool.Parse(_configuration["Smtp:EnableSsl"] ?? "true");

        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(fromEmail))
        {
            _logger.LogWarning("Chưa cấu hình SMTP (mục Smtp trong appsettings.json). Không thể gửi email tới {Email}.", toEmail);
            throw new InvalidOperationException("Hệ thống email chưa được cấu hình.");
        }

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(username, password),
            EnableSsl = enableSsl
        };

        var body = $@"
            <div style='font-family:Segoe UI,Arial,sans-serif;max-width:480px;margin:auto'>
                <h2 style='color:#0d6efd'>Xin chào {WebUtility.HtmlEncode(toName)},</h2>
                <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản SoftSync của bạn.</p>
                <p>Nhấn nút bên dưới để đặt mật khẩu mới (liên kết có hiệu lực trong 1 giờ):</p>
                <p style='text-align:center;margin:24px 0'>
                    <a href='{resetLink}' style='background:#0d6efd;color:#fff;padding:12px 24px;border-radius:6px;text-decoration:none'>Đặt lại mật khẩu</a>
                </p>
                <p>Nếu bạn không yêu cầu, hãy bỏ qua email này.</p>
                <p style='color:#888;font-size:12px'>SoftSync - Nền tảng phát triển kỹ năng mềm</p>
            </div>";

        using var message = new MailMessage
        {
            From = new MailAddress(fromEmail!, fromName),
            Subject = "SoftSync - Yêu cầu đặt lại mật khẩu",
            Body = body,
            IsBodyHtml = true
        };
        message.To.Add(new MailAddress(toEmail, toName));

        await client.SendMailAsync(message);
    }
}