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
    public DbSet<TheoryLessonProgress> TheoryLessonProgresses { get; set; }
    public DbSet<MentorConversation> MentorConversations { get; set; }
    public DbSet<MentorMessage> MentorMessages { get; set; }


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

        modelBuilder.Entity<TheoryLessonProgress>()
            .HasIndex(p => new { p.UserId, p.TheoryLessonId }).IsUnique();

        modelBuilder.Entity<MentorConversation>()
            .HasIndex(c => new { c.UserId, c.MentorId }).IsUnique();

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

        // 3. Case Studies
        modelBuilder.Entity<CaseStudy>().HasData(
            new CaseStudy { Id = 1, Title = "Group Communication", Scenario = "Thành viên nhóm của bạn không đóng góp. Bạn sẽ làm gì?", SkillId = 2 },
            new CaseStudy { Id = 2, Title = "Missed Deadline", Scenario = "Bạn nhận ra rằng bạn sẽ trễ hạn vào ngày mai. Hành động đầu tiên của bạn là gì?", SkillId = 1 },
            new CaseStudy { Id = 3, Title = "Critical Decision", Scenario = "Bạn phải đưa ra một quyết định quan trọng nhưng thiếu thông tin. Bạn sẽ làm gì?", SkillId = 3 }
        );

        modelBuilder.Entity<CaseStudyOption>().HasData(
            new CaseStudyOption { Id = 1, CaseStudyId = 1, OptionText = "Làm công việc của họ.", IsRecommended = false, Feedback = "Điều này dẫn đến kiệt sức và không giải quyết được vấn đề động lực nhóm." },
            new CaseStudyOption { Id = 2, CaseStudyId = 1, OptionText = "Nói chuyện riêng với họ để hiểu tình hình.", IsRecommended = true, Feedback = "Giao tiếp trực tiếp và đồng cảm là chìa khóa." },
            new CaseStudyOption { Id = 3, CaseStudyId = 2, OptionText = "Làm việc cả đêm và hy vọng điều tốt nhất.", IsRecommended = false, Feedback = "Rủi ro và không quản lý được kỳ vọng." },
            new CaseStudyOption { Id = 4, CaseStudyId = 2, OptionText = "Thông báo ngay cho các bên liên quan và đề xuất một lịch trình mới.", IsRecommended = true, Feedback = "Minh bạch và lập kế hoạch chủ động là điều cần thiết." },
            new CaseStudyOption { Id = 5, CaseStudyId = 3, OptionText = "Đợi thêm thông tin trước khi hành động.", IsRecommended = false, Feedback = "Điều này có thể dẫn đến mất cơ hội." },
            new CaseStudyOption { Id = 6, CaseStudyId = 3, OptionText = "Thu thập thông tin có sẵn và đưa ra quyết định tốt nhất có thể.", IsRecommended = true, Feedback = "Đánh giá rủi ro và đưa ra quyết định dựa trên dữ liệu hiện có là cách tiếp cận thực tế." }
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
