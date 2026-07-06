using Microsoft.EntityFrameworkCore;
using SoftSync.DAL.Data;
using SoftSync.DAL.Entities;

namespace SoftSync.DAL.Repositories;

public interface IRoadmapRepository : IRepository<RoadmapItem>
{
    Task<IEnumerable<RoadmapItem>> GetByUserIdAsync(int userId);
}
public class RoadmapRepository : Repository<RoadmapItem>, IRoadmapRepository
{
    public RoadmapRepository(SoftSyncDbContext context) : base(context) { }
    public async Task<IEnumerable<RoadmapItem>> GetByUserIdAsync(int userId)
    {
        return await _dbSet.Where(r => r.UserId == userId).OrderBy(r => r.WeekNumber).ToListAsync();
    }
}
