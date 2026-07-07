using Microsoft.EntityFrameworkCore;
using SoftSync.DAL.Entities;

namespace SoftSync.DAL.Repositories;

public interface IMiniGameAttemptRepository
{
    Task AddAsync(MiniGameAttempt attempt);
    Task SaveChangesAsync();
    Task<List<MiniGameAttempt>> GetRecentByUserAndSkillAsync(int userId, int skillId, int take); // MỚI
}
public class MiniGameAttemptRepository : IMiniGameAttemptRepository
{
    private readonly Data.SoftSyncDbContext _context;
    public MiniGameAttemptRepository(Data.SoftSyncDbContext context) => _context = context;

    public async Task AddAsync(MiniGameAttempt attempt) => await _context.MiniGameAttempts.AddAsync(attempt);
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    public async Task<List<MiniGameAttempt>> GetRecentByUserAndSkillAsync(int userId, int skillId, int take)
    {
        return await _context.MiniGameAttempts
            .Where(a => a.UserId == userId && a.MiniGame.SkillId == skillId)
            .OrderByDescending(a => a.PlayedAt)
            .Take(take)
            .Include(a => a.MiniGame)
            .ToListAsync();
    }
}