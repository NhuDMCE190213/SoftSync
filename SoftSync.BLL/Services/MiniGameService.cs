using SoftSync.BLL.Interfaces;
using SoftSync.Common.Dtos;
using SoftSync.Common.Dtos.MiniGame;
using SoftSync.DAL.Entities;
using SoftSync.DAL.Repositories;
using System.Text.Json;

namespace SoftSync.BLL.Services;

public class MiniGameService : IMiniGameService
{
    private readonly IMiniGameRepository _miniGameRepo;

    private readonly IMiniGameAttemptRepository _attemptRepo;

    public MiniGameService(IMiniGameRepository miniGameRepo, IMiniGameAttemptRepository attemptRepo)
    {
        _miniGameRepo = miniGameRepo;
        _attemptRepo = attemptRepo;
    }

    public async Task<List<MiniGameDto>> GetMiniGamesBySkillIdAsync(int skillId)
    {
        var all = await _miniGameRepo.GetAllWithQuestionsAsync();
        return all.Where(g => g.SkillId == skillId)
            .Select(g => new MiniGameDto { Id = g.Id, Name = g.Name, Description = g.Description })
            .ToList();
    }

    public async Task<List<MiniGameQuestionDto>> GetRandomQuestionsAsync(int miniGameId)
    {
        var questions = await _miniGameRepo.GetRandomQuestionsAsync(miniGameId, 5);
        return questions.Select(q => new MiniGameQuestionDto
        {
            Id = q.Id,
            ScenarioText = q.ScenarioText,
            ContextNote = q.ContextNote,
            Options = q.Options.Select(o => new MiniGameOptionDto { Id = o.Id, OptionText = o.OptionText }).ToList()
        }).ToList();
    }

    public async Task<int> SubmitAttemptAsync(int userId, int miniGameId, List<MiniGameAnswerDto> answers)
    {
        var optionIds = answers.Select(a => a.OptionId).ToList();
        var options = await _miniGameRepo.GetOptionsByIdsAsync(optionIds);

        int totalScore = options.Sum(o => o.Points);

        var answersMap = answers.ToDictionary(a => a.QuestionId, a => a.OptionId);

        var attempt = new MiniGameAttempt
        {
            UserId = userId,
            MiniGameId = miniGameId,
            TotalScore = totalScore,
            PlayedAt = DateTime.UtcNow,
            AnswersJson = JsonSerializer.Serialize(answersMap)
        };

        await _attemptRepo.AddAsync(attempt);
        await _attemptRepo.SaveChangesAsync();

        return totalScore;
    }
}