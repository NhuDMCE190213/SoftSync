namespace SoftSync.DAL.Entities;

public class UserSkillSelection
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int SkillId { get; set; }
    public Skill Skill { get; set; } = null!;
}
