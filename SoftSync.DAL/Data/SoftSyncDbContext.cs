using Microsoft.EntityFrameworkCore;
using SoftSync.Common.Enums;
using SoftSync.Common.Security;
using SoftSync.DAL.Entities;

namespace SoftSync.DAL.Data;

public class SoftSyncDbContext : DbContext
{
    public SoftSyncDbContext(DbContextOptions<SoftSyncDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<UserSkillSelection> UserSkillSelections { get; set; }
    public DbSet<AssessmentQuestion> AssessmentQuestions { get; set; }
    public DbSet<AssessmentOption> AssessmentOptions { get; set; }
    public DbSet<AssessmentResult> AssessmentResults { get; set; }
    public DbSet<RoadmapItem> RoadmapItems { get; set; }
    public DbSet<CaseStudy> CaseStudies { get; set; }
    public DbSet<CaseStudyOption> CaseStudyOptions { get; set; }
    public DbSet<ProgressLog> ProgressLogs { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<Mentor> Mentors { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    public DbSet<TheoryLesson> TheoryLessons { get; set; }
    public DbSet<MiniGame> MiniGames { get; set; }
    public DbSet<MiniGameOption> MiniGameOptions { get; set; }
    public DbSet<MiniGameQuestion> MiniGameQuestions { get; set; }
    public DbSet<EntryTestQuestion> EntryTestQuestions { get; set; }
    public DbSet<EntryTestOption> EntryTestOptions { get; set; }
    public DbSet<EntryTestResult> EntryTestResults { get; set; }
    public DbSet<MiniGameAttempt> MiniGameAttempts { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Many-to-Many: User <-> Skill (Selections)
        modelBuilder.Entity<UserSkillSelection>()
            .HasKey(us => new { us.UserId, us.SkillId });

        modelBuilder.Entity<UserSkillSelection>()
            .HasOne(us => us.User)
            .WithMany(u => u.SkillSelections)
            .HasForeignKey(us => us.UserId);

        modelBuilder.Entity<UserSkillSelection>()
            .HasOne(us => us.Skill)
            .WithMany()
            .HasForeignKey(us => us.SkillId);

        // Seed Data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // 1. Skills
        modelBuilder.Entity<Skill>().HasData(
            new Skill { Id = 1, Name = "Time Management", Description = "Quản lý thời gian hiệu quả.", IconName = "bi-clock" },
            new Skill { Id = 2, Name = "Communication", Description = "Giao tiếp hiệu quả.", IconName = "bi-chat-dots" },
            new Skill { Id = 3, Name = "Critical Thinking", Description = "Tư duy phản biện.", IconName = "bi-lightbulb" }
        );

        // 2. Assessment Questions (Communication)
        //modelBuilder.Entity<AssessmentQuestion>().HasData(
        //    new AssessmentQuestion { Id = 1, SkillId = 1, QuestionText = "How do you handle a conflict during a group discussion?", Type = QuestionType.Scenario },
        //    new AssessmentQuestion { Id = 2, SkillId = 1, QuestionText = "How important is active listening in a conversation?", Type = QuestionType.MultipleChoice }
        //);

        //modelBuilder.Entity<AssessmentOption>().HasData(
        //    new AssessmentOption { Id = 1, QuestionId = 1, OptionText = "Stay quiet to avoid more conflict.", ScoreValue = 1 },
        //    new AssessmentOption { Id = 2, QuestionId = 1, OptionText = "Listen to all sides and suggest a compromise.", ScoreValue = 5 },
        //    new AssessmentOption { Id = 3, QuestionId = 1, OptionText = "Insist on my point of view.", ScoreValue = 2 },
        //    new AssessmentOption { Id = 4, QuestionId = 2, OptionText = "Not important.", ScoreValue = 1 },
        //    new AssessmentOption { Id = 5, QuestionId = 2, OptionText = "Very important.", ScoreValue = 5 }
        //);

        // 3. Case Studies
        modelBuilder.Entity<CaseStudy>().HasData(
            new CaseStudy { Id = 1, Title = "Group Communication", Scenario = "Your team member is not contributing. What do you do?", SkillId = 1 },
            new CaseStudy { Id = 2, Title = "Missed Deadline", Scenario = "You realize you will miss a deadline tomorrow. What is your first action?", SkillId = 3 }
        );

        modelBuilder.Entity<CaseStudyOption>().HasData(
            new CaseStudyOption { Id = 1, CaseStudyId = 1, OptionText = "Do their work myself.", IsRecommended = false, Feedback = "This leads to burnout and doesn't solve the team dynamic issue." },
            new CaseStudyOption { Id = 2, CaseStudyId = 1, OptionText = "Talk to them privately to understand their situation.", IsRecommended = true, Feedback = "Direct, empathetic communication is key." },
            new CaseStudyOption { Id = 3, CaseStudyId = 2, OptionText = "Work all night and hope for the best.", IsRecommended = false, Feedback = "Risky and doesn't manage expectations." },
            new CaseStudyOption { Id = 4, CaseStudyId = 2, OptionText = "Inform the stakeholders immediately and propose a new timeline.", IsRecommended = true, Feedback = "Transparency and proactive planning are essential." }
        );

        // 4. Demo Users — MỚI: thêm Email + PasswordHash
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                FullName = "Nguyễn Văn A",
                Email = "demo@softsync.vn",
                PasswordHash = PasswordHasher.Hash("Demo@123"),
                Age = 20,
                Role = UserRole.Student,
                Goal = "Cải thiện kỹ năng giao tiếp",
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            }
        );

        // 5. Mentors
        modelBuilder.Entity<Mentor>().HasData(
            new Mentor { Id = 1, Name = "Dr. Katherine", Expertise = "Leadership & Soft Skills", ShortBio = "15 years of experience in corporate training.", AvatarUrl = "https://i.pravatar.cc/150?u=katherine" },
            new Mentor { Id = 2, Name = "John Smith", Expertise = "Problem Solving", ShortBio = "Former Google engineer, expert in algorithms and thinking.", AvatarUrl = "https://i.pravatar.cc/150?u=john" }
        );
    }
}
