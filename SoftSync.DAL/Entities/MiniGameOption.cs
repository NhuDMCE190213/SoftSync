using System.ComponentModel.DataAnnotations;

namespace SoftSync.DAL.Entities;

// Đáp án — dùng chung cho cả 3 game dạng trắc nghiệm (Game 1,2,3 + Priority/Eisenhower)
public class MiniGameOption
{
    [Key] public int Id { get; set; }
    public int MiniGameQuestionId { get; set; }
    public MiniGameQuestion MiniGameQuestion { get; set; } = null!;
    [Required] public string OptionText { get; set; } = string.Empty;
    public int Points { get; set; }        // theo thang điểm của data: Tối ưu=10, Trung lập=5, Tiêu cực=0
    public string Feedback { get; set; } = string.Empty;
}
