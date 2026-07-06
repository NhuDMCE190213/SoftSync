using SoftSync.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace SoftSync.DAL.Entities;

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
