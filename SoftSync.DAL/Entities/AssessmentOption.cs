using System.ComponentModel.DataAnnotations;

namespace SoftSync.DAL.Entities;

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
