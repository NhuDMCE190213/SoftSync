using System.ComponentModel.DataAnnotations;

namespace SoftSync.DAL.Entities;

// Ngân hàng câu hỏi/tình huống của 1 game (20 câu/game)
public class MiniGameQuestion
{
    [Key] public int Id { get; set; }
    public int MiniGameId { get; set; }
    public MiniGame MiniGame { get; set; } = null!;
    [Required] public string ScenarioText { get; set; } = string.Empty; // tình huống/câu gốc
    public string? ContextNote { get; set; }  // ví dụ: bối cảnh nhân vật Javier, Val/Lee...

    public ICollection<MiniGameOption> Options { get; set; } = new List<MiniGameOption>();
}
