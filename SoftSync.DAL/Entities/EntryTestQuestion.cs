using System.ComponentModel.DataAnnotations;

namespace SoftSync.DAL.Entities;

public class EntryTestQuestion
{
    [Key] public int Id { get; set; }
    public int OrderIndex { get; set; }      // Câu 1 -> 24, cố định thứ tự nhóm theo trụ
    public int SkillId { get; set; }         // Câu 1-8: QLTG, 9-16: Giao tiếp, 17-24: Tư duy phản biện
    public Skill Skill { get; set; } = null!;
    [Required] public string QuestionText { get; set; } = string.Empty;

    public ICollection<EntryTestOption> Options { get; set; } = new List<EntryTestOption>();
}
