using System.ComponentModel.DataAnnotations;

namespace SoftSync.DAL.Entities;

public class EntryTestOption
{
    [Key] public int Id { get; set; }
    public int EntryTestQuestionId { get; set; }
    public EntryTestQuestion EntryTestQuestion { get; set; } = null!;
    [Required] public string OptionText { get; set; } = string.Empty;
    public int Level { get; set; } // 1-4, tương ứng "mức 1 (bị động)" -> "mức 4 (chủ động nhất)"
}
