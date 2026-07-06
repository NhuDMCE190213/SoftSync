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
