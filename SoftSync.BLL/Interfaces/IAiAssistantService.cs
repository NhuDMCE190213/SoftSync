namespace SoftSync.BLL.Interfaces;

public interface IAiAssistantService
{
    Task<string> GetReplyAsync(string userMessage, int userId);
}
