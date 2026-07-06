using SoftSync.DAL.Data;
using SoftSync.DAL.Entities;

namespace SoftSync.DAL.Repositories
{
    public interface ISkillRepository : IRepository<Skill> { }
    public class SkillRepository : Repository<Skill>, ISkillRepository
    {
        public SkillRepository(SoftSyncDbContext context) : base(context) { }
    }
}
