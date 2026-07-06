using SoftSync.Common.Enums;

namespace SoftSync.Common.Dtos;

public class MessageDto
{
    public int Id { get; set; }
    public ChatSender Sender { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
