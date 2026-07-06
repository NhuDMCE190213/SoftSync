using SoftSync.Common.Enums;

namespace SoftSync.BLL.Services
{
    // SoftSync.BLL/Services/EntryTestScoringService.cs
    public static class EntryTestScoringRules
    {
        // Bảng tra CỨNG theo mục II/III của tài liệu — KHÔNG cho AI thay đổi
        public static AssessmentLevel GetPillarLevel(int score) => score switch
        {
            >= 8 and <= 14 => AssessmentLevel.Passive,
            >= 15 and <= 20 => AssessmentLevel.Developing,
            >= 21 and <= 26 => AssessmentLevel.Proactive,
            >= 27 and <= 32 => AssessmentLevel.Exceptional,
            _ => throw new ArgumentOutOfRangeException(nameof(score))
        };

        public static AssessmentLevel GetOverallLevel(int score) => score switch
        {
            >= 24 and <= 41 => AssessmentLevel.Passive,
            >= 42 and <= 59 => AssessmentLevel.Developing,
            >= 60 and <= 77 => AssessmentLevel.Proactive,
            >= 78 and <= 96 => AssessmentLevel.Exceptional,
            _ => throw new ArgumentOutOfRangeException(nameof(score))
        };
    }
}
