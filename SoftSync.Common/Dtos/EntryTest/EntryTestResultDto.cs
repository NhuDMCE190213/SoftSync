namespace SoftSync.Common.Dtos.EntryTest;

public class EntryTestResultDto
{
    public int TotalScore { get; set; }
    public string OverallLevel { get; set; } = string.Empty;
    public int TimeManagementScore { get; set; }
    public string TimeManagementLevel { get; set; } = string.Empty;
    public int CommunicationScore { get; set; }
    public string CommunicationLevel { get; set; } = string.Empty;
    public int CriticalThinkingScore { get; set; }
    public string CriticalThinkingLevel { get; set; } = string.Empty;
}
