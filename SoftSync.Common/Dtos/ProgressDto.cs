namespace SoftSync.Common.Dtos;

public class ProgressDto
{
    public int UserId { get; set; }
    public int SkillId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public int PercentComplete { get; set; }
    public DateTime UpdatedAt { get; set; }
}
