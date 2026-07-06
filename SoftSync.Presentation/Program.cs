using SoftSync.Presentation.Components;
using Microsoft.EntityFrameworkCore;
using SoftSync.DAL.Data;
using SoftSync.DAL.Repositories;
using SoftSync.BLL.Interfaces;
using SoftSync.BLL.Services;
using SoftSync.BLL.Services.Fake;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using SoftSync.Common.Dtos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 1. Database Configuration
var connectionString = builder.Configuration.GetConnectionString("SoftSyncDb");
builder.Services.AddDbContext<SoftSyncDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Register Repositories (DAL)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISkillRepository, SkillRepository>();
builder.Services.AddScoped<IAssessmentRepository, AssessmentRepository>();
builder.Services.AddScoped<IRoadmapRepository, RoadmapRepository>();
builder.Services.AddScoped<ICaseStudyRepository, CaseStudyRepository>();
builder.Services.AddScoped<IProgressRepository, ProgressRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IMentorRepository, MentorRepository>();
builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>(); // MỚI

// 3. Register AI Services (BLL - Mocked)
builder.Services.AddScoped<IAiAssessmentService, FakeAiAssessmentService>();
builder.Services.AddScoped<IAiAssistantService, FakeAiAssistantService>();
builder.Services.AddScoped<IAiRoadmapService, FakeAiRoadmapService>();

// 4. Register Business Services (BLL)
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IAssessmentService, AssessmentService>();
builder.Services.AddScoped<IRoadmapService, RoadmapService>();
builder.Services.AddScoped<IProgressService, ProgressService>();
builder.Services.AddScoped<ICaseStudyService, CaseStudyService>();
builder.Services.AddScoped<IMentorService, MentorService>();
builder.Services.AddScoped<IAuthService, AuthService>();          // MỚI
builder.Services.AddScoped<IEmailService, SmtpEmailService>();    // MỚI

// 5. HttpClient for future AI integration
builder.Services.AddHttpClient("AiApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AiApi:BaseUrl"] ?? "http://localhost:5000");
});

// 6. MỚI: Authentication / Authorization
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.Name = "SoftSync.Auth";
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication(); // MỚI - phải nằm trước UseAuthorization
app.UseAuthorization();  // MỚI

// ====== MỚI: endpoint xử lý đăng nhập / đăng ký / quên & đặt lại mật khẩu ======
// Dùng <form method="post"> HTML thường (không qua SignalR) để có thể gọi
// HttpContext.SignInAsync/SignOutAsync an toàn trong Blazor Server.

app.MapPost("/api/auth/login", async (HttpContext http, IAuthService authService) =>
{
    var form = await http.Request.ReadFormAsync();
    var dto = new LoginRequestDto { Email = form["Email"].ToString(), Password = form["Password"].ToString() };
    var returnUrl = form["ReturnUrl"].ToString();

    var result = await authService.LoginAsync(dto);
    if (!result.Success || result.User == null)
        return Results.Redirect($"/login?error={Uri.EscapeDataString(result.Message)}&returnUrl={Uri.EscapeDataString(returnUrl)}");

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, result.User.Id.ToString()),
        new(ClaimTypes.Name, result.User.FullName),
        new(ClaimTypes.Email, result.User.Email)
    };
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity),
        new AuthenticationProperties { IsPersistent = true });

    return Results.Redirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
});

app.MapPost("/api/auth/register", async (HttpContext http, IAuthService authService) =>
{
    var form = await http.Request.ReadFormAsync();
    var dto = new RegisterRequestDto
    {
        FullName = form["FullName"].ToString(),
        Email = form["Email"].ToString(),
        Password = form["Password"].ToString(),
        ConfirmPassword = form["ConfirmPassword"].ToString(),
        Age = int.TryParse(form["Age"], out var age) ? age : 0,
        Goal = form["Goal"].ToString()
    };

    var result = await authService.RegisterAsync(dto);
    if (!result.Success || result.User == null)
        return Results.Redirect($"/register?error={Uri.EscapeDataString(result.Message)}");

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, result.User.Id.ToString()),
        new(ClaimTypes.Name, result.User.FullName),
        new(ClaimTypes.Email, result.User.Email)
    };
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity),
        new AuthenticationProperties { IsPersistent = true });

    return Results.Redirect("/");
});

app.MapPost("/api/auth/forgot-password", async (HttpContext http, IAuthService authService) =>
{
    var form = await http.Request.ReadFormAsync();
    var dto = new ForgotPasswordRequestDto { Email = form["Email"].ToString() };
    var baseUrl = $"{http.Request.Scheme}://{http.Request.Host}";

    var result = await authService.ForgotPasswordAsync(dto, baseUrl);
    return Results.Redirect($"/forgot-password?message={Uri.EscapeDataString(result.Message)}");
});

app.MapPost("/api/auth/reset-password", async (HttpContext http, IAuthService authService) =>
{
    var form = await http.Request.ReadFormAsync();
    var dto = new ResetPasswordRequestDto
    {
        Token = form["Token"].ToString(),
        NewPassword = form["NewPassword"].ToString(),
        ConfirmPassword = form["ConfirmPassword"].ToString()
    };

    var result = await authService.ResetPasswordAsync(dto);
    if (!result.Success)
        return Results.Redirect($"/reset-password?token={Uri.EscapeDataString(dto.Token)}&error={Uri.EscapeDataString(result.Message)}");

    return Results.Redirect($"/login?message={Uri.EscapeDataString(result.Message)}");
});

app.MapPost("/api/auth/logout", async (HttpContext http) =>
{
    await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});

app.MapPost("/api/user/update-profile", async (HttpContext http, IUserService userService) =>
{
    var userIdClaim = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
        return Results.Redirect("/login");

    var form = await http.Request.ReadFormAsync();
    var fullName = form["FullName"].ToString();
    var age = int.TryParse(form["Age"], out var a) ? a : 0;
    var goal = form["Goal"].ToString();

    var result = await userService.UpdateProfileAsync(userId, fullName, age, goal);
    if (!result.Success || result.User == null)
        return Results.Redirect($"/profile?error={Uri.EscapeDataString(result.Message)}");

    // Re-issue cookie để tên mới hiển thị ngay trên menu (không cần đăng nhập lại)
    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, result.User.Id.ToString()),
        new(ClaimTypes.Name, result.User.FullName),
        new(ClaimTypes.Email, result.User.Email)
    };
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity),
        new AuthenticationProperties { IsPersistent = true });

    return Results.Redirect($"/profile?message={Uri.EscapeDataString(result.Message)}");
}).RequireAuthorization();

app.MapPost("/api/auth/change-password", async (HttpContext http, IAuthService authService) =>
{
    var userIdClaim = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
        return Results.Redirect("/login");

    var form = await http.Request.ReadFormAsync();
    var result = await authService.ChangePasswordAsync(
        userId,
        form["CurrentPassword"].ToString(),
        form["NewPassword"].ToString(),
        form["ConfirmPassword"].ToString());

    return Results.Redirect(result.Success
        ? $"/change-password?message={Uri.EscapeDataString(result.Message)}"
        : $"/change-password?error={Uri.EscapeDataString(result.Message)}");
}).RequireAuthorization();
// ====== HẾT PHẦN MỚI ======

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();