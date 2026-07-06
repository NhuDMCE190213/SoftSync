namespace SoftSync.Common.Dtos.Roadmap;

public class RoadmapDto
{
    public int UserId { get; set; }
    public List<RoadmapItemDto> Items { get; set; } = new();
}
