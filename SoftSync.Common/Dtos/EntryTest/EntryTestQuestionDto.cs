namespace SoftSync.Common.Dtos.EntryTest;

public class EntryTestQuestionDto
{
    public int Id { get; set; }
    public int OrderIndex { get; set; }
    public int SkillId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public List<EntryTestOptionDto> Options { get; set; } = new();
}
