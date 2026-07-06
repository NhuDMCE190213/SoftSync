namespace SoftSync.Common.Dtos.CaseStudy;

public class CaseStudyOptionDto
{
    public int Id { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public bool IsRecommended { get; set; }
    public string Feedback { get; set; } = string.Empty;
}
