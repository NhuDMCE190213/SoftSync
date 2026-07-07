using SoftSync.BLL.Interfaces;
using SoftSync.Common.Dtos.EntryTest;
using SoftSync.Common.Enums;
using SoftSync.DAL.Entities;
using SoftSync.DAL.Repositories;

namespace SoftSync.BLL.Services;

public class EntryTestService : IEntryTestService
{
    private readonly IEntryTestRepository _repo;
    private readonly IUserRepository _userRepo;
    // Giả định SkillId: 1 = TimeManagement, 2 = Communication, 3 = CriticalThinking
    private const int SKILL_TIME = 1, SKILL_COMM = 2, SKILL_CRITICAL = 3;

    public EntryTestService(IEntryTestRepository repo, IUserRepository userRepo)
    {
        _repo = repo;
        _userRepo = userRepo;
    }

    public async Task<List<EntryTestQuestionDto>> GetQuestionsAsync(int userId)
    {
        var questions = await _repo.GetAllQuestionsAsync();
        var selectedSkillIds = await _userRepo.GetSelectedSkillIdsAsync(userId);

        // Nếu user đã chọn skill cụ thể -> chỉ hỏi đúng những skill đó.
        // Nếu chưa chọn skill nào (phòng thủ) -> hỏi đủ 3 trụ như cũ.
        var filtered = selectedSkillIds.Any()
            ? questions.Where(q => selectedSkillIds.Contains(q.SkillId)).ToList()
            : questions;

        return filtered
            .OrderBy(q => q.OrderIndex)
            .Select(q => new EntryTestQuestionDto
            {
                Id = q.Id,
                OrderIndex = q.OrderIndex,
                SkillId = q.SkillId,
                QuestionText = q.QuestionText,
                Options = q.Options.Select(o => new EntryTestOptionDto
                {
                    Id = o.Id,
                    OptionText = o.OptionText,
                    Level = o.Level
                }).ToList()
            }).ToList();
    }

    public async Task<EntryTestResultDto> SubmitAsync(int userId, List<EntryTestAnswerDto> answers)
    {
        var questions = await _repo.GetAllQuestionsAsync();

        int timeScore = 0, commScore = 0, criticalScore = 0;
        bool timeAnswered = false, commAnswered = false, criticalAnswered = false;

        foreach (var ans in answers)
        {
            var question = questions.First(q => q.Id == ans.QuestionId);
            var option = question.Options.First(o => o.Id == ans.OptionId);

            if (question.SkillId == SKILL_TIME) { timeScore += option.Level; timeAnswered = true; }
            else if (question.SkillId == SKILL_COMM) { commScore += option.Level; commAnswered = true; }
            else if (question.SkillId == SKILL_CRITICAL) { criticalScore += option.Level; criticalAnswered = true; }
        }

        int totalScore = timeScore + commScore + criticalScore;
        bool allThreePillarsAnswered = timeAnswered && commAnswered && criticalAnswered;

        var result = new EntryTestResult
        {
            UserId = userId,
            TotalScore = totalScore,
            // Overall level chỉ có ý nghĩa khi user làm đủ cả 3 trụ (24 câu).
            // Nếu chỉ chọn 1-2 skill, "tổng thể" không áp dụng được -> dùng Developing làm placeholder trung tính.
            OverallLevel = allThreePillarsAnswered
                ? EntryTestScoringRules.GetOverallLevel(totalScore)
                : AssessmentLevel.Developing,
            TimeManagementScore = timeScore,
            TimeManagementLevel = timeAnswered ? EntryTestScoringRules.GetPillarLevel(timeScore) : AssessmentLevel.Developing,
            CommunicationScore = commScore,
            CommunicationLevel = commAnswered ? EntryTestScoringRules.GetPillarLevel(commScore) : AssessmentLevel.Developing,
            CriticalThinkingScore = criticalScore,
            CriticalThinkingLevel = criticalAnswered ? EntryTestScoringRules.GetPillarLevel(criticalScore) : AssessmentLevel.Developing,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.SaveResultAsync(result);
        return MapToDto(result);
    }

    public async Task<List<EntryTestQuestionDto>> GetQuestionsAsync(int userId, List<int>? skillIdsOverride = null)
    {
        var questions = await _repo.GetAllQuestionsAsync();
        var selectedSkillIds = skillIdsOverride ?? await _userRepo.GetSelectedSkillIdsAsync(userId);
        var filtered = selectedSkillIds.Any() ? questions.Where(q => selectedSkillIds.Contains(q.SkillId)).ToList() : questions;
        return filtered.OrderBy(q => q.OrderIndex).Select(q => new EntryTestQuestionDto
        {
            Id = q.Id,
            OrderIndex = q.OrderIndex,
            SkillId = q.SkillId,
            QuestionText = q.QuestionText,
            Options = q.Options.Select(o => new EntryTestOptionDto { Id = o.Id, OptionText = o.OptionText, Level = o.Level }).ToList()
        }).ToList();
    }

    public async Task<EntryTestResultDto?> GetResultAsync(int userId)
    {
        var result = await _repo.GetLatestResultAsync(userId);
        return result == null ? null : MapToDto(result);
    }

    private static EntryTestResultDto MapToDto(EntryTestResult r) => new()
    {
        TotalScore = r.TotalScore,
        OverallLevel = r.OverallLevel.ToString(),
        TimeManagementScore = r.TimeManagementScore,
        TimeManagementLevel = r.TimeManagementLevel.ToString(),
        CommunicationScore = r.CommunicationScore,
        CommunicationLevel = r.CommunicationLevel.ToString(),
        CriticalThinkingScore = r.CriticalThinkingScore,
        CriticalThinkingLevel = r.CriticalThinkingLevel.ToString()
    };
}