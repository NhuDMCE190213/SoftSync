using SoftSync.BLL.Interfaces;
using SoftSync.Common.Dtos;
using SoftSync.Common.Enums;

namespace SoftSync.BLL.Services.Fake;

public class FakeAiAssessmentService : IAiAssessmentService
{
    public Task<AssessmentResultDto> EvaluateAsync(List<UserAnswerDto> answers)
    {
        int score = (answers.Count * 23) % 100;
        var level = score < 50 ? AssessmentLevel.Weak : score < 80 ? AssessmentLevel.Average : AssessmentLevel.Good;

        return Task.FromResult(new AssessmentResultDto
        {
            SkillId = 1,
            Score = score,
            Level = level,
            CreatedAt = DateTime.UtcNow
        });
    }
}

public class FakeAiAssistantService : IAiAssistantService
{
    public Task<string> GetReplyAsync(string userMessage, int userId)
    {
        string reply = "Chao ban! Toi la tro ly SoftSync AI. ";
        if (userMessage.ToLower().Contains("giao tiep"))
            reply += "De cai thien ky nang giao tiep, hay bat dau bang viec lang nghe chu dong hon nhe.";
        else if (userMessage.ToLower().Contains("lo trinh"))
            reply += "Toi da tao lo trinh hoc tap ca nhan hoa cho ban trong tab Roadmap roi day!";
        else
            reply += $"Cam on ban da nhan tin: '{userMessage}'. Toi san sang dong hanh cung ban tren con duong phat trien ky nang mem.";

        return Task.FromResult(reply);
    }
}

public class FakeAiRoadmapService : IAiRoadmapService
{
    public Task<RoadmapDto> GenerateRoadmapAsync(int userId, List<string> weakSkills)
    {
        var items = new List<RoadmapItemDto>();

        // Each skill gets exactly 3 weeks. There is no general overview week.
        foreach (var skill in weakSkills)
        {
            var skillVi = ToVietnameseSkillName(skill);
            var skillLabel = ToRoadmapSkillLabel(skill);

            items.Add(new RoadmapItemDto
            {
                WeekNumber = 1,
                SkillName = skillLabel,
                Title = $"{skillLabel} - Nền tảng",
                Description = $"Nắm lý thuyết cốt lõi của {skillVi.ToLower()} và xem những tình huống minh họa đầu tiên.",
                IsCompleted = false
            });

            items.Add(new RoadmapItemDto
            {
                WeekNumber = 2,
                SkillName = skillLabel,
                Title = $"{skillLabel} - Thực hành",
                Description = $"Áp dụng {skillVi.ToLower()} vào các bài luyện tập và tình huống thực tế.",
                IsCompleted = false
            });

            items.Add(new RoadmapItemDto
            {
                WeekNumber = 3,
                SkillName = skillLabel,
                Title = $"{skillLabel} - Ứng dụng",
                Description = $"Ôn lại, củng cố và chuẩn bị chuyển sang phần game thực hành cho {skillVi.ToLower()}.",
                IsCompleted = false
            });
        }

        return Task.FromResult(new RoadmapDto { UserId = userId, Items = items });
    }

    private static string ToVietnameseSkillName(string englishName) => englishName switch
    {
        "Communication" => "Kỹ năng Giao tiếp",
        "Time Management" => "Kỹ năng Quản lý thời gian",
        "Critical Thinking" => "Kỹ năng Tư duy phản biện",
        _ => englishName
    };

    private static string ToRoadmapSkillLabel(string englishName) => englishName switch
    {
        "Communication" => "Giao tiếp",
        "Time Management" => "Quản lý thời gian",
        "Critical Thinking" => "Tư duy phản biện",
        _ => englishName
    };
}
