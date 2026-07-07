using System.ComponentModel.DataAnnotations;

namespace SoftSync.DAL.Entities;

public class RoadmapItem
{
    [Key]
    public int Id { get; set; }
    public int? SkillId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int WeekNumber { get; set; }
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public int? TheoryLessonId { get; set; }
    public int? MiniGameId { get; set; }
    public bool IsRemedial { get; set; } = false;
}
