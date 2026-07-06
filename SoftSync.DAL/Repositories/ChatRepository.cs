using Microsoft.EntityFrameworkCore;
using SoftSync.DAL.Entities;

namespace SoftSync.DAL.Repositories;

public interface IChatRepository : IRepository<ChatMessage>
{
    Task<IEnumerable<ChatMessage>> GetByUserIdAsync(int userId);
}
public class ChatRepository : Repository<ChatMessage>, IChatRepository
{
    public ChatRepository(Data.SoftSyncDbContext context) : base(context) { }
    public async Task<IEnumerable<ChatMessage>> GetByUserIdAsync(int userId)
    {
        return await _dbSet.Where(c => c.UserId == userId).OrderBy(c => c.CreatedAt).ToListAsync();
    }
}
