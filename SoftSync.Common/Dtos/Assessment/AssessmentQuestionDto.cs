namespace SoftSync.Common.Dtos.Assessment;

public class AssessmentQuestionDto // Local DTO for BLL to UI
{
    public int Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public int SkillId { get; set; }
    public List<AssessmentOptionDto> Options { get; set; } = new();
}
