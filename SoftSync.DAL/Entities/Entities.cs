using SoftSync.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace SoftSync.DAL.Entities;

public class User
{
    [Key]
    public int Id { get; set; }
    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;
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
