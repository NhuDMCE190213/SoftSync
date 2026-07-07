using SoftSync.BLL.Interfaces;
using SoftSync.Common.Dtos;
using SoftSync.Common.Dtos.Assessment;
using SoftSync.Common.Dtos.Roadmap;
using SoftSync.Common.Enums;
using SoftSync.DAL.Repositories;

namespace SoftSync.BLL.Services.Fake;

// TODO [AI-TEAM: Kiệt/Chánh] - Thay FakeAiAssessmentService bằng implementation
// gọi API AI thật tại đây. Giữ nguyên interface IAiAssessmentService (ở SoftSync.BLL),
// chỉ cần đổi class đăng ký trong DI tại Program.cs.
public class FakeAiAssessmentService : IAiAssessmentService
{
    private readonly IEntryTestRepository _entryTestRepo;
    public FakeAiAssessmentService(IEntryTestRepository entryTestRepo) => _entryTestRepo = entryTestRepo;

    public async Task<AssessmentResultDto> EvaluateAsync(List<UserAnswerDto> answers)
    {
        int score = (answers.Count * 23) % 100;
        var level = score < 25 ? AssessmentLevel.Passive
                  : score < 50 ? AssessmentLevel.Developing
                  : score < 75 ? AssessmentLevel.Proactive
                  : AssessmentLevel.Exceptional;

        var questions = await _entryTestRepo.GetAllQuestionsAsync();
        int skillId = answers.Any()
            ? questions.First(q => q.Id == answers[0].QuestionId).SkillId
            : 0;

        return new AssessmentResultDto
        {
            SkillId = skillId,
            Score = score,
            Level = level,
            CreatedAt = DateTime.UtcNow
        };
    }
}

// TODO [AI-TEAM: Kiệt/Chánh] - Thay FakeAiAssistantService bằng implementation
// gọi API chatbot thật tại đây.
public class FakeAiAssistantService : IAiAssistantService
{
    public Task<string> GetReplyAsync(string userMessage, int userId)
    {
        string reply = "Chào bạn! Tôi là trợ lý SoftSync AI. ";
        if (userMessage.ToLower().Contains("giao tiếp"))
            reply += "Để cải thiện kỹ năng giao tiếp, hãy bắt đầu bằng việc lắng nghe chủ động hơn nhé.";
        else if (userMessage.ToLower().Contains("lộ trình"))
            reply += "Tôi đã tạo lộ trình học tập cá nhân hóa cho bạn trong tab Roadmap rồi đấy!";
        else
            reply += $"Cảm ơn bạn đã nhắn tin: '{userMessage}'. Tôi sẵn sàng đồng hành cùng bạn trên con đường phát triển kỹ năng mềm.";

        return Task.FromResult(reply);
    }
}

// TODO [AI-TEAM: Kiệt/Chánh] - Thay FakeAiRoadmapService bằng implementation
// sinh lộ trình thật từ LLM/AI.
public class FakeAiRoadmapService : IAiRoadmapService
{
    public Task<RoadmapDto> GenerateRoadmapAsync(int userId, List<SkillWeaknessDto> weaknesses)
    {
        var week = 1;
        var items = new List<RoadmapItemDto>
        {
            new RoadmapItemDto { SkillId = null, WeekNumber = week++, Title = "Khám phá bản thân", Description = "Hoàn thành bài đánh giá năng lực và xác định mục tiêu.", IsCompleted = false }
        };

        // Fake nhưng vẫn tôn trọng input thật thay vì trả cứng
        foreach (var w in weaknesses.OrderBy(w => w.Level))
        {
            items.Add(new RoadmapItemDto
            {
                SkillId = w.SkillId,
                WeekNumber = week++,
                Title = $"Kỹ năng {w.SkillName}",
                Description = $"Cải thiện {w.SkillName} (mức hiện tại: {w.Level}).",
                IsCompleted = false
            });
        }

        items.Add(new RoadmapItemDto { SkillId = null, WeekNumber = week, Title = "Tổng kết", Description = "Xem lại tiến độ và nhận feedback từ AI Mentor.", IsCompleted = false });

        return Task.FromResult(new RoadmapDto { UserId = userId, Items = items });
    }
}
