using SoftSync.Common.Dtos.Roadmap;

namespace SoftSync.BLL.Interfaces;

public interface IAiRoadmapService
{
    Task<RoadmapDto> GenerateRoadmapAsync(int userId, List<string> weakSkills);
}
