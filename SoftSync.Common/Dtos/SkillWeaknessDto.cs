using SoftSync.Common.Enums;

namespace SoftSync.Common.Dtos.Roadmap;

public class SkillWeaknessDto
{
    public int SkillId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public int Score { get; set; }
    public AssessmentLevel Level { get; set; }
    public List<string> MistakePatterns { get; set; } = new(); // MỚI
}