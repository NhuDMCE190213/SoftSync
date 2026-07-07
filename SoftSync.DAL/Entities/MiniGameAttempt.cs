using System.ComponentModel.DataAnnotations;

namespace SoftSync.DAL.Entities;

// Lượt chơi của người dùng (lưu để AI cá nhân hóa + chống chơi lại spam)
public class MiniGameAttempt
{
    [Key] public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int MiniGameId { get; set; }
    public MiniGame MiniGame { get; set; } = null!;
    public int TotalScore { get; set; }
    public int MaxScore { get; set; }
    public DateTime PlayedAt { get; set; } = DateTime.UtcNow;
    public string AnswersJson { get; set; } = string.Empty; // lưu QuestionId->OptionId đã chọn, phục vụ AI phân tích
}
