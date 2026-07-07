using SoftSync.BLL.Interfaces;
using SoftSync.Common.Dtos;
using SoftSync.DAL.Repositories;

namespace SoftSync.BLL.Services;

public class ProgressService : IProgressService
{
    private readonly IProgressRepository _progressRepo;
    public ProgressService(IProgressRepository progressRepo) => _progressRepo = progressRepo;
    public async Task<IEnumerable<ProgressDto>> GetUserProgressAsync(int userId)
    {
        var logs = await _progressRepo.GetByUserIdAsync(userId);
        return logs.Select(l => new ProgressDto { UserId = l.UserId, SkillId = l.SkillId, SkillName = l.Skill.Name, PercentComplete = l.PercentComplete, UpdatedAt = l.UpdatedAt });
    }
    public async Task UpsertProgressAsync(int userId, int skillId, int percentDelta)
    {
        var log = await _progressRepo.GetOrCreateAsync(userId, skillId);
        log.PercentComplete = Math.Clamp(log.PercentComplete + percentDelta, 0, 100);
        log.UpdatedAt = DateTime.UtcNow;
        await _progressRepo.SaveChangesAsync();
    }
}
