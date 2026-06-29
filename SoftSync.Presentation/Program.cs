using SoftSync.Presentation.Components;
using Microsoft.EntityFrameworkCore;
using SoftSync.DAL.Data;
using SoftSync.DAL.Repositories;
using SoftSync.BLL.Interfaces;
using SoftSync.BLL.Services;
using SoftSync.BLL.Services.Fake;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

// 5. HttpClient for future AI integration
builder.Services.AddHttpClient("AiApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AiApi:BaseUrl"] ?? "http://localhost:5000");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
