using Microsoft.EntityFrameworkCore;
using SoftSync.DAL.Data;
using SoftSync.DAL.Entities;

namespace SoftSync.DAL.Repositories
{
    public interface ITheoryLessonRepository : IRepository<TheoryLesson>
    {
        Task<List<TheoryLesson>> GetBySkillIdAsync(int skillId);
    }
    public class TheoryLessonRepository : Repository<TheoryLesson>, ITheoryLessonRepository
    {
        public TheoryLessonRepository(SoftSyncDbContext context) : base(context) { }
        public async Task<List<TheoryLesson>> GetBySkillIdAsync(int skillId)
            => await _dbSet.Where(t => t.SkillId == skillId).OrderBy(t => t.OrderIndex).ToListAsync();
    }
}
