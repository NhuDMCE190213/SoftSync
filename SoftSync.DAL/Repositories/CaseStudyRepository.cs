using Microsoft.EntityFrameworkCore;
using SoftSync.DAL.Entities;

namespace SoftSync.DAL.Repositories;

public interface ICaseStudyRepository : IRepository<CaseStudy>
{
    Task<IEnumerable<CaseStudy>> GetBySkillIdAsync(int skillId);
    Task<CaseStudy?> GetWithDetailsAsync(int id);
}
public class CaseStudyRepository : Repository<CaseStudy>, ICaseStudyRepository
{
    public CaseStudyRepository(Data.SoftSyncDbContext context) : base(context) { }
    public async Task<IEnumerable<CaseStudy>> GetBySkillIdAsync(int skillId)
    {
        return await _dbSet.Where(cs => cs.SkillId == skillId).Include(cs => cs.Options).ToListAsync();
    }
    public async Task<CaseStudy?> GetWithDetailsAsync(int id)
    {
        return await _dbSet.Include(cs => cs.Options).FirstOrDefaultAsync(cs => cs.Id == id);
    }
}
