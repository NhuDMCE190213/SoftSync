using SoftSync.BLL.Interfaces;
using SoftSync.Common.Dtos;
using SoftSync.Common.Dtos.EntryTest;
using SoftSync.Common.Enums;
using SoftSync.DAL.Entities;
using SoftSync.DAL.Repositories;

namespace SoftSync.BLL.Services;

public class EntryTestService : IEntryTestService
{
    private readonly IEntryTestRepository _repo;
    // Giả định SkillId: 1 = TimeManagement, 2 = Communication, 3 = CriticalThinking
    private const int SKILL_TIME = 1, SKILL_COMM = 2, SKILL_CRITICAL = 3;

    public EntryTestService(IEntryTestRepository repo) => _repo = repo;

    public async Task<List<EntryTestQuestionDto>> GetQuestionsAsync()
    {
        var questions = await _repo.GetAllQuestionsAsync();
        return questions.Select(q => new EntryTestQuestionDto
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

        foreach (var ans in answers)
        {
            var question = questions.First(q => q.Id == ans.QuestionId);
            var option = question.Options.First(o => o.Id == ans.OptionId);

            if (question.SkillId == SKILL_TIME) timeScore += option.Level;
            else if (question.SkillId == SKILL_COMM) commScore += option.Level;
            else if (question.SkillId == SKILL_CRITICAL) criticalScore += option.Level;
        }

        int totalScore = timeScore + commScore + criticalScore;

        var result = new EntryTestResult
        {
            UserId = userId,
            TotalScore = totalScore,
            OverallLevel = EntryTestScoringRules.GetOverallLevel(totalScore),
            TimeManagementScore = timeScore,
            TimeManagementLevel = EntryTestScoringRules.GetPillarLevel(timeScore),
            CommunicationScore = commScore,
            CommunicationLevel = EntryTestScoringRules.GetPillarLevel(commScore),
            CriticalThinkingScore = criticalScore,
            CriticalThinkingLevel = EntryTestScoringRules.GetPillarLevel(criticalScore),
            CreatedAt = DateTime.UtcNow
        };

        await _repo.SaveResultAsync(result);

        return MapToDto(result);
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