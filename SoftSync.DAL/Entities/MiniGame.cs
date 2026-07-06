using SoftSync.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace SoftSync.DAL.Entities;

// 1 mini game thuộc 1 kỹ năng (VD: Game 1 - Chọn câu trả lời phù hợp)
public class MiniGame
{
    [Key] public int Id { get; set; }
    public int SkillId { get; set; }
    public Skill Skill { get; set; } = null!;
    [Required, MaxLength(150)] public string Name { get; set; } = string.Empty;
    public MiniGameType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public int QuestionsPerRound { get; set; } = 5;  // random 5/20 theo yêu cầu

    public ICollection<MiniGameQuestion> Questions { get; set; } = new List<MiniGameQuestion>();
}
