using Microsoft.EntityFrameworkCore;
using SoftSync.DAL.Entities;

namespace SoftSync.DAL.Repositories;

public interface IMiniGameAttemptRepository
{
    Task AddAsync(MiniGameAttempt attempt);
    Task SaveChangesAsync();
    Task<List<MiniGameAttempt>> GetRecentByUserAndSkillAsync(int userId, int skillId, int take); // MỚI
    Task<int> CountDistinctPassedAsync(int userId, int skillId, double passingRatio); // MỚI - đếm số minigame KHÁC NHAU đã pass, tránh farm điểm bằng cách chơi lại
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

    public async Task<int> CountDistinctPassedAsync(int userId, int skillId, double passingRatio)
    {
        return await _context.MiniGameAttempts
            .Where(a => a.UserId == userId
                        && a.MiniGame.SkillId == skillId
                        && a.MaxScore > 0
                        && (double)a.TotalScore / a.MaxScore >= passingRatio)
            .Select(a => a.MiniGameId)
            .Distinct()
            .CountAsync();
    }
}