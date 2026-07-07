using Microsoft.EntityFrameworkCore;
using SoftSync.DAL.Entities;

namespace SoftSync.DAL.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<List<int>> GetSelectedSkillIdsAsync(int userId);
        Task ReplaceSkillSelectionsAsync(int userId, List<int> skillIds);
        Task<List<int>> AddNewSkillSelectionsAsync(int userId, List<int> skillIds);
    }
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(Data.SoftSyncDbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email)
            => await _dbSet.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

        public async Task<bool> EmailExistsAsync(string email)
            => await _dbSet.AnyAsync(u => u.Email.ToLower() == email.ToLower());

        public async Task<List<int>> GetSelectedSkillIdsAsync(int userId)
            => await _dbSet.Where(u => u.Id == userId)
            .SelectMany(u => u.SkillSelections.Select(s => s.SkillId))
            .ToListAsync();
        public async Task ReplaceSkillSelectionsAsync(int userId, List<int> skillIds)
        {
            var existing = await _context.Set<UserSkillSelection>()
                .Where(s => s.UserId == userId).ToListAsync();
            _context.Set<UserSkillSelection>().RemoveRange(existing);

            foreach (var sid in skillIds.Distinct())
                await _context.Set<UserSkillSelection>().AddAsync(new UserSkillSelection { UserId = userId, SkillId = sid });

            await _context.SaveChangesAsync();
        }
        public async Task<List<int>> AddNewSkillSelectionsAsync(int userId, List<int> skillIds)
        {
            var existing = await _context.Set<UserSkillSelection>()
                .Where(s => s.UserId == userId).Select(s => s.SkillId).ToListAsync();

            var toAdd = skillIds.Except(existing).Distinct().ToList();
            foreach (var sid in toAdd)
                await _context.Set<UserSkillSelection>().AddAsync(new UserSkillSelection { UserId = userId, SkillId = sid });

            await _context.SaveChangesAsync();
            return toAdd; // chỉ trả về skill thực sự mới, để entry test biết cần hỏi gì
        }
    }
}
