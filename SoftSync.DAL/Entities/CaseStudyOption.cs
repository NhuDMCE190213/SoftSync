using System.ComponentModel.DataAnnotations;

namespace SoftSync.DAL.Entities;

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
