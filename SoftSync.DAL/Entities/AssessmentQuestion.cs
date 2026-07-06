using SoftSync.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace SoftSync.DAL.Entities;

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
