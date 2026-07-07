using Microsoft.EntityFrameworkCore;
using SoftSync.DAL.Entities;

namespace SoftSync.DAL.Repositories;

public interface IProgressRepository : IRepository<ProgressLog>
{
    Task<IEnumerable<ProgressLog>> GetByUserIdAsync(int userId);
    Task<ProgressLog> GetOrCreateAsync(int userId, int skillId);
}
public class ProgressRepository : Repository<ProgressLog>, IProgressRepository
{
    public ProgressRepository(Data.SoftSyncDbContext context) : base(context) { }
    public async Task<IEnumerable<ProgressLog>> GetByUserIdAsync(int userId)
    {
        return await _dbSet.Where(p => p.UserId == userId).Include(p => p.Skill).ToListAsync();
    }
    public async Task<ProgressLog> GetOrCreateAsync(int userId, int skillId)
    {
        var log = await _dbSet.FirstOrDefaultAsync(p => p.UserId == userId && p.SkillId == skillId);
        if (log == null)
        {
            log = new ProgressLog { UserId = userId, SkillId = skillId, PercentComplete = 0, UpdatedAt = DateTime.UtcNow };
            await _dbSet.AddAsync(log);
        }
        return log;
    }
}
