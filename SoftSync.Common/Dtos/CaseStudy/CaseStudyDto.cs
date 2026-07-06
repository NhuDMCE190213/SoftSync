namespace SoftSync.Common.Dtos.CaseStudy;

public class CaseStudyDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Scenario { get; set; } = string.Empty;
    public int SkillId { get; set; }
    public List<CaseStudyOptionDto> Options { get; set; } = new();
}
