using SoftSync.Common.Dtos;

namespace SoftSync.BLL.Interfaces;

public interface IProgressService
{
    Task<IEnumerable<ProgressDto>> GetUserProgressAsync(int userId);
    Task UpsertProgressAsync(int userId, int skillId, int percentDelta);
}
