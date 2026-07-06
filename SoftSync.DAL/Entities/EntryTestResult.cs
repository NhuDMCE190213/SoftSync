using SoftSync.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace SoftSync.DAL.Entities;

public class EntryTestResult
{
    [Key] public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int TotalScore { get; set; }              // 24-96
    public AssessmentLevel OverallLevel { get; set; }
    public int TimeManagementScore { get; set; }      // 8-32
    public AssessmentLevel TimeManagementLevel { get; set; }
    public int CommunicationScore { get; set; }
    public AssessmentLevel CommunicationLevel { get; set; }
    public int CriticalThinkingScore { get; set; }
    public AssessmentLevel CriticalThinkingLevel { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
