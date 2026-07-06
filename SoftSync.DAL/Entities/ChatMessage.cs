using SoftSync.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace SoftSync.DAL.Entities;

public class ChatMessage
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public ChatSender Sender { get; set; }
    [Required]
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
