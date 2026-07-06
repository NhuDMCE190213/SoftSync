using SoftSync.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace SoftSync.DAL.Entities;

public class User
{
    [Key]
    public int Id { get; set; }
    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;
    [Required, MaxLength(150)]
    public string Email { get; set; } = string.Empty;         

    [Required]
    public string PasswordHash { get; set; } = string.Empty;   
    public int Age { get; set; }
    public UserRole Role { get; set; }
    [MaxLength(500)]
    public string Goal { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<UserSkillSelection> SkillSelections { get; set; } = new List<UserSkillSelection>();
    public ICollection<AssessmentResult> AssessmentResults { get; set; } = new List<AssessmentResult>();
    public ICollection<RoadmapItem> RoadmapItems { get; set; } = new List<RoadmapItem>();
    public ICollection<ProgressLog> ProgressLogs { get; set; } = new List<ProgressLog>();
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
}

// Nội dung lý thuyết + video cho từng kỹ năng
public class TheoryLesson
{
    [Key] public int Id { get; set; }
    public int SkillId { get; set; }
    public Skill Skill { get; set; } = null!;
    [Required, MaxLength(200)] public string Title { get; set; } = string.Empty;
    [Required] public string ContentMarkdown { get; set; } = string.Empty; // nội dung lý thuyết dạng markdown
    public string? VideoUrl { get; set; }        // link video giảng dạy do các cô quay
    public int OrderIndex { get; set; }          // thứ tự bài học trong lộ trình (Tuần 1, Tuần 2...)
}

// 1 mini game thuộc 1 kỹ năng (VD: Game 1 - Chọn câu trả lời phù hợp)
public class MiniGame
{
    [Key] public int Id { get; set; }
    public int SkillId { get; set; }
    public Skill Skill { get; set; } = null!;
    [Required, MaxLength(150)] public string Name { get; set; } = string.Empty;
    public MiniGameType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public int QuestionsPerRound { get; set; } = 5;  // random 5/20 theo yêu cầu

    public ICollection<MiniGameQuestion> Questions { get; set; } = new List<MiniGameQuestion>();
}

// Ngân hàng câu hỏi/tình huống của 1 game (20 câu/game)
public class MiniGameQuestion
{
    [Key] public int Id { get; set; }
    public int MiniGameId { get; set; }
    public MiniGame MiniGame { get; set; } = null!;
    [Required] public string ScenarioText { get; set; } = string.Empty; // tình huống/câu gốc
    public string? ContextNote { get; set; }  // ví dụ: bối cảnh nhân vật Javier, Val/Lee...

    public ICollection<MiniGameOption> Options { get; set; } = new List<MiniGameOption>();
}

// Đáp án — dùng chung cho cả 3 game dạng trắc nghiệm (Game 1,2,3 + Priority/Eisenhower)
public class MiniGameOption
{
    [Key] public int Id { get; set; }
    public int MiniGameQuestionId { get; set; }
    public MiniGameQuestion MiniGameQuestion { get; set; } = null!;
    [Required] public string OptionText { get; set; } = string.Empty;
    public int Points { get; set; }        // theo thang điểm của data: Tối ưu=10, Trung lập=5, Tiêu cực=0
    public string Feedback { get; set; } = string.Empty;
}

// Lượt chơi của người dùng (lưu để AI cá nhân hóa + chống chơi lại spam)
public class MiniGameAttempt
{
    [Key] public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int MiniGameId { get; set; }
    public MiniGame MiniGame { get; set; } = null!;
    public int TotalScore { get; set; }
    public DateTime PlayedAt { get; set; } = DateTime.UtcNow;
    public string AnswersJson { get; set; } = string.Empty; // lưu QuestionId->OptionId đã chọn, phục vụ AI phân tích
}

// MỚI: token dùng cho tính năng quên mật khẩu
public class PasswordResetToken
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    [Required, MaxLength(200)]
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Skill
{
    [Key]
    public int Id { get; set; }
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    [MaxLength(50)]
    public string IconName { get; set; } = string.Empty;

    public ICollection<AssessmentQuestion> Questions { get; set; } = new List<AssessmentQuestion>();
    public ICollection<CaseStudy> CaseStudies { get; set; } = new List<CaseStudy>();
}

public class UserSkillSelection
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int SkillId { get; set; }
    public Skill Skill { get; set; } = null!;
}

public class AssessmentQuestion
{
    [Key]
    public int Id { get; set; }
    public int SkillId { get; set; }
    public Skill Skill { get; set; } = null!;
    [Required]
    public string QuestionText { get; set; } = string.Empty;
    public QuestionType Type { get; set; }

    public ICollection<AssessmentOption> Options { get; set; } = new List<AssessmentOption>();
}

public class AssessmentOption
{
    [Key]
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public AssessmentQuestion Question { get; set; } = null!;
    [Required]
    public string OptionText { get; set; } = string.Empty;
    public int ScoreValue { get; set; }
}

public class AssessmentResult
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int SkillId { get; set; }
    public Skill Skill { get; set; } = null!;
    public int Score { get; set; }
    public AssessmentLevel Level { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class RoadmapItem
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int WeekNumber { get; set; }
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}

public class CaseStudy
{
    [Key]
    public int Id { get; set; }
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    [Required]
    public string Scenario { get; set; } = string.Empty;
    public int SkillId { get; set; }
    public Skill Skill { get; set; } = null!;

    public ICollection<CaseStudyOption> Options { get; set; } = new List<CaseStudyOption>();
}

public class CaseStudyOption
{
    [Key]
    public int Id { get; set; }
    public int CaseStudyId { get; set; }
    public CaseStudy CaseStudy { get; set; } = null!;
    [Required]
    public string OptionText { get; set; } = string.Empty;
    public bool IsRecommended { get; set; }
    public string Feedback { get; set; } = string.Empty;
}

public class ProgressLog
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int SkillId { get; set; }
    public Skill Skill { get; set; } = null!;
    public int PercentComplete { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class ChatMessage
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public ChatSender Sender { get; set; }
    [Required]
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Mentor
{
    [Key]
    public int Id { get; set; }
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required, MaxLength(100)]
    public string Expertise { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string ShortBio { get; set; } = string.Empty;
}

public class EntryTestQuestion
{
    [Key] public int Id { get; set; }
    public int OrderIndex { get; set; }      // Câu 1 -> 24, cố định thứ tự nhóm theo trụ
    public int SkillId { get; set; }         // Câu 1-8: QLTG, 9-16: Giao tiếp, 17-24: Tư duy phản biện
    public Skill Skill { get; set; } = null!;
    [Required] public string QuestionText { get; set; } = string.Empty;

    public ICollection<EntryTestOption> Options { get; set; } = new List<EntryTestOption>();
}

public class EntryTestOption
{
    [Key] public int Id { get; set; }
    public int EntryTestQuestionId { get; set; }
    public EntryTestQuestion EntryTestQuestion { get; set; } = null!;
    [Required] public string OptionText { get; set; } = string.Empty;
    public int Level { get; set; } // 1-4, tương ứng "mức 1 (bị động)" -> "mức 4 (chủ động nhất)"
}

public class EntryTestResult
{
    [Key] public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int TotalScore { get; set; }              // 24-96
    public AssessmentLevel OverallLevel { get; set; }
    public int TimeManagementScore { get; set; }      // 8-32
    public AssessmentLevel TimeManagementLevel { get; set; }
    public int CommunicationScore { get; set; }
    public AssessmentLevel CommunicationLevel { get; set; }
    public int CriticalThinkingScore { get; set; }
    public AssessmentLevel CriticalThinkingLevel { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
