using SoftSync.DAL.Entities;

namespace SoftSync.DAL.Repositories;

public interface IMiniGameAttemptRepository
{
    Task AddAsync(MiniGameAttempt attempt);
    Task SaveChangesAsync();
}
public class MiniGameAttemptRepository : IMiniGameAttemptRepository
{
    private readonly Data.SoftSyncDbContext _context;
    public MiniGameAttemptRepository(Data.SoftSyncDbContext context) => _context = context;

    public async Task AddAsync(MiniGameAttempt attempt) => await _context.MiniGameAttempts.AddAsync(attempt);
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}