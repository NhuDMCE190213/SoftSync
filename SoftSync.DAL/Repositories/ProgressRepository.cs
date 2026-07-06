using Microsoft.EntityFrameworkCore;
using SoftSync.DAL.Entities;

namespace SoftSync.DAL.Repositories;

public interface IProgressRepository : IRepository<ProgressLog>
{
    Task<IEnumerable<ProgressLog>> GetByUserIdAsync(int userId);
}
public class ProgressRepository : Repository<ProgressLog>, IProgressRepository
{
    public ProgressRepository(Data.SoftSyncDbContext context) : base(context) { }
    public async Task<IEnumerable<ProgressLog>> GetByUserIdAsync(int userId)
    {
        return await _dbSet.Where(p => p.UserId == userId).Include(p => p.Skill).ToListAsync();
    }
}
