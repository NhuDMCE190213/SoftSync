using System.ComponentModel.DataAnnotations;

namespace SoftSync.DAL.Entities;

public class TheoryLessonProgress
{
    [Key] public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int TheoryLessonId { get; set; }
    public TheoryLesson TheoryLesson { get; set; } = null!;
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
}