using SoftSync.Common.Enums;

namespace SoftSync.Common.Dtos.Assessment;

public class AssessmentResultDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int SkillId { get; set; }
    public int Score { get; set; }
    public AssessmentLevel Level { get; set; }
    public DateTime CreatedAt { get; set; }
    public string SkillName { get; set; } = string.Empty;
}
