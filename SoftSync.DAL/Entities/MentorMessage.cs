using System.ComponentModel.DataAnnotations;

namespace SoftSync.DAL.Entities;

public enum MentorMessageSender { User, Mentor }

public class MentorMessage
{
    [Key] public int Id { get; set; }
    public int ConversationId { get; set; }
    public MentorConversation Conversation { get; set; } = null!;
    public MentorMessageSender SenderType { get; set; }
    [Required] public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;
}