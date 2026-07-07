using SoftSync.Common.Dtos.Roadmap;

namespace SoftSync.BLL.Interfaces;

public interface IRoadmapService
{
    Task<RoadmapDto> GetUserRoadmapAsync(int userId);
    Task GenerateRoadmapAsync(int userId);
    Task MarkCompleteAsync(int itemId);
}
