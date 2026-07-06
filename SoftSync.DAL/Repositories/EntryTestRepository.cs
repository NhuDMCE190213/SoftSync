using Microsoft.EntityFrameworkCore;
using SoftSync.DAL.Entities;

namespace SoftSync.DAL.Repositories
{
    public interface IEntryTestRepository
    {
        Task<List<EntryTestQuestion>> GetAllQuestionsAsync();
        Task SaveResultAsync(EntryTestResult result);
        Task<EntryTestResult?> GetLatestResultAsync(int userId);
    }

    public class EntryTestRepository : IEntryTestRepository
    {
        private readonly Data.SoftSyncDbContext _context;
        public EntryTestRepository(Data.SoftSyncDbContext context) => _context = context;

        public async Task<List<EntryTestQuestion>> GetAllQuestionsAsync()
        {
            return await _context.EntryTestQuestions
                .Include(q => q.Options)
                .OrderBy(q => q.OrderIndex)
                .ToListAsync();
        }

        public async Task SaveResultAsync(EntryTestResult result)
        {
            await _context.EntryTestResults.AddAsync(result);
            await _context.SaveChangesAsync();
        }

        public async Task<EntryTestResult?> GetLatestResultAsync(int userId)
        {
            return await _context.EntryTestResults
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync();
        }
    }
}
