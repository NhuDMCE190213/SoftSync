using System.ComponentModel.DataAnnotations;

namespace SoftSync.DAL.Entities;

public class MentorConversation
{
    [Key] public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int MentorId { get; set; }
    public Mentor Mentor { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<MentorMessage> Messages { get; set; } = new List<MentorMessage>();
}