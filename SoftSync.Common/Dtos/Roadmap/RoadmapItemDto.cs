namespace SoftSync.Common.Dtos.Roadmap;

public class RoadmapItemDto
{
    public int Id { get; set; }
    public int? SkillId { get; set; }
    public int WeekNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public bool IsLocked { get; set; } // MỚI
}
