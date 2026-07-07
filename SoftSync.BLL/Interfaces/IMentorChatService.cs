namespace SoftSync.BLL.Interfaces;

public interface IMentorChatService
{
    Task<List<(string SenderType, string Content, DateTime CreatedAt)>> GetConversationAsync(int userId, int mentorId);
    Task SendMessageAsync(int userId, int mentorId, string content);
}