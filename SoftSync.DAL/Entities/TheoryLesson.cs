using System.ComponentModel.DataAnnotations;

namespace SoftSync.DAL.Entities;

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
