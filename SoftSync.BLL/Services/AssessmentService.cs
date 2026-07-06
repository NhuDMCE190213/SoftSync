using SoftSync.BLL.Interfaces;
using SoftSync.Common.Dtos;
using SoftSync.Common.Dtos.Assessment;
using SoftSync.DAL.Entities;
using SoftSync.DAL.Repositories;

namespace SoftSync.BLL.Services;

public class AssessmentService : IAssessmentService
{
    private readonly IAssessmentRepository _assessmentRepo;
    private readonly IUserRepository _userRepo;
    private readonly IAiAssessmentService _aiService;

    public AssessmentService(IAssessmentRepository assessmentRepo, IUserRepository userRepo, IAiAssessmentService aiService)
    {
        _assessmentRepo = assessmentRepo;
        _userRepo = userRepo;
        _aiService = aiService;
    }

    public async Task<IEnumerable<AssessmentQuestionDto>> GetAssessmentQuestionsAsync(int userId)
    {
        // For MVP, get all or by selected skills
        var questions = await _assessmentRepo.GetQuestionsBySkillIdsAsync(new List<int> { 1, 2, 3, 4, 5, 6, 7 });
        return questions.Select(q => new AssessmentQuestionDto
        {
            Id = q.Id,
            QuestionText = q.QuestionText,
            SkillId = q.SkillId,
            Options = q.Options.Select(o => new AssessmentOptionDto { Id = o.Id, OptionText = o.OptionText }).ToList()
        });
    }

    public async Task SubmitAssessmentAsync(int userId, List<UserAnswerDto> answers)
    {
        // Call AI Service (Fake for now)
        var result = await _aiService.EvaluateAsync(answers);
        result.UserId = userId;

        // Save to DB
        await _assessmentRepo.SaveResultAsync(new AssessmentResult
        {
            UserId = userId,
            SkillId = result.SkillId,
            Score = result.Score,
            Level = result.Level,
            CreatedAt = DateTime.UtcNow
        });
    }

    public async Task<IEnumerable<AssessmentResultDto>> GetLatestResultsAsync(int userId)
    {
        var results = await _assessmentRepo.GetResultsByUserIdAsync(userId);
        return results.Select(r => new AssessmentResultDto
        {
            Id = r.Id,
            UserId = r.UserId,
            SkillId = r.SkillId,
            SkillName = r.Skill.Name,
            Score = r.Score,
            Level = r.Level,
            CreatedAt = r.CreatedAt
        });
    }
}
