using System.ComponentModel.DataAnnotations;

namespace SoftSync.DAL.Entities;

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
