using Microsoft.EntityFrameworkCore;
using SoftSync.DAL.Entities;

namespace SoftSync.DAL.Repositories
{
    public interface IMiniGameRepository : IRepository<MiniGame>
    {
        Task<IEnumerable<MiniGame>> GetAllWithQuestionsAsync();
        Task<List<MiniGameQuestion>> GetRandomQuestionsAsync(int miniGameId, int count);
        Task<List<MiniGameOption>> GetOptionsByIdsAsync(List<int> optionIds); 
        Task<List<MiniGameQuestion>> GetQuestionsByIdsAsync(List<int> questionIds);
        Task<List<MiniGame>> GetBySkillIdAsync(int skillId);
    }

    public class MiniGameRepository : Repository<MiniGame>, IMiniGameRepository
    {
        public MiniGameRepository(Data.SoftSyncDbContext context) : base(context) { }
        public async Task<IEnumerable<MiniGame>> GetAllWithQuestionsAsync()
        {
            return await _dbSet.Include(mg => mg.Questions).ToListAsync();
        }
        public async Task<List<MiniGameQuestion>> GetRandomQuestionsAsync(int miniGameId, int count)
        {
            return await _context.MiniGameQuestions
                .Where(q => q.MiniGameId == miniGameId)
                .Include(q => q.Options)
                .OrderBy(q => Guid.NewGuid())   // SQL Server dịch thành ORDER BY NEWID()
                .Take(count)
                .ToListAsync();
        }
        public async Task<List<MiniGameOption>> GetOptionsByIdsAsync(List<int> optionIds)
        {
            return await _context.MiniGameOptions
                .Where(o => optionIds.Contains(o.Id))
                .ToListAsync();
        }

        public async Task<List<MiniGameQuestion>> GetQuestionsByIdsAsync(List<int> questionIds)
        {
            return await _context.MiniGameQuestions
                .Where(q => questionIds.Contains(q.Id))
                .ToListAsync();
        }

        public async Task<List<MiniGame>> GetBySkillIdAsync(int skillId)
        {
            return await _dbSet.Where(mg => mg.SkillId == skillId).ToListAsync();
        }
    }
}