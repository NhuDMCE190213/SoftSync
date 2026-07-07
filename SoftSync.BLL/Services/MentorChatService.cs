using SoftSync.BLL.Interfaces;
using SoftSync.DAL.Entities;
using SoftSync.DAL.Repositories;

namespace SoftSync.BLL.Services;

public class MentorChatService : IMentorChatService
{
    private readonly IMentorChatRepository _repo;
    public MentorChatService(IMentorChatRepository repo) => _repo = repo;

    public async Task<List<(string, string, DateTime)>> GetConversationAsync(int userId, int mentorId)
    {
        var conv = await _repo.GetOrCreateConversationAsync(userId, mentorId);
        var messages = await _repo.GetMessagesAsync(conv.Id);
        return messages.Select(m => (m.SenderType.ToString(), m.Content, m.CreatedAt)).ToList();
    }

    public async Task SendMessageAsync(int userId, int mentorId, string content)
    {
        var conv = await _repo.GetOrCreateConversationAsync(userId, mentorId);
        await _repo.AddMessageAsync(new MentorMessage
        {
            ConversationId = conv.Id,
            SenderType = MentorMessageSender.User,
            Content = content
        });
        await _repo.SaveChangesAsync();
    }
}