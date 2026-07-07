using Microsoft.EntityFrameworkCore;
using SoftSync.DAL.Entities;

namespace SoftSync.DAL.Repositories;

public interface IMentorChatRepository
{
    Task<MentorConversation> GetOrCreateConversationAsync(int userId, int mentorId);
    Task<List<MentorMessage>> GetMessagesAsync(int conversationId);
    Task AddMessageAsync(MentorMessage message);
    Task SaveChangesAsync();
}

public class MentorChatRepository : IMentorChatRepository
{
    private readonly Data.SoftSyncDbContext _context;
    public MentorChatRepository(Data.SoftSyncDbContext context) => _context = context;

    public async Task<MentorConversation> GetOrCreateConversationAsync(int userId, int mentorId)
    {
        var conv = await _context.Set<MentorConversation>()
            .FirstOrDefaultAsync(c => c.UserId == userId && c.MentorId == mentorId);
        if (conv == null)
        {
            conv = new MentorConversation { UserId = userId, MentorId = mentorId };
            await _context.Set<MentorConversation>().AddAsync(conv);
            await _context.SaveChangesAsync();
        }
        return conv;
    }

    public async Task<List<MentorMessage>> GetMessagesAsync(int conversationId)
        => await _context.Set<MentorMessage>()
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();

    public async Task AddMessageAsync(MentorMessage message)
        => await _context.Set<MentorMessage>().AddAsync(message);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}