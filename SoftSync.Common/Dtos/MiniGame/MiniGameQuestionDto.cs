namespace SoftSync.Common.Dtos.MiniGame;

public class MiniGameQuestionDto
{
    public int Id { get; set; }
    public string ScenarioText { get; set; } = string.Empty;
    public string? ContextNote { get; set; }
    public List<MiniGameOptionDto> Options { get; set; } = new();
}
