using Microsoft.EntityFrameworkCore;
using SoftSync.DAL.Data;
using SoftSync.DAL.Entities;

namespace SoftSync.DAL.Repositories
{
    public interface ITheoryLessonProgressRepository
    {
        Task<bool> ExistsAsync(int userId, int theoryLessonId);
        Task AddAsync(TheoryLessonProgress entity);
        Task SaveChangesAsync();
        Task<int> CountCompletedAsync(int userId, List<int> theoryLessonIds);
    }

    public class TheoryLessonProgressRepository : ITheoryLessonProgressRepository
    {
        private readonly SoftSyncDbContext _context;
        public TheoryLessonProgressRepository(SoftSyncDbContext context) => _context = context;

        public Task<bool> ExistsAsync(int userId, int theoryLessonId) =>
            _context.TheoryLessonProgresses.AnyAsync(p => p.UserId == userId && p.TheoryLessonId == theoryLessonId);

        public async Task AddAsync(TheoryLessonProgress entity) => await _context.TheoryLessonProgresses.AddAsync(entity);
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
        public async Task<int> CountCompletedAsync(int userId, List<int> theoryLessonIds)
        {
            if (!theoryLessonIds.Any()) return 0;
            return await _context.TheoryLessonProgresses
                .CountAsync(p => p.UserId == userId && theoryLessonIds.Contains(p.TheoryLessonId));
        }
    }
}
