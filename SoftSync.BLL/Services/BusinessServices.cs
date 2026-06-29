using SoftSync.BLL.Interfaces;
using SoftSync.Common.Dtos;
using SoftSync.DAL.Entities;
using SoftSync.DAL.Repositories;

namespace SoftSync.BLL.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    public UserService(IUserRepository userRepo) => _userRepo = userRepo;

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user == null) return null;
        return new UserDto { Id = user.Id, FullName = user.FullName, Age = user.Age, Goal = user.Goal, CreatedAt = user.CreatedAt };
    }

    public async Task<UserDto> CreateUserAsync(UserDto dto)
    {
        var user = new User { FullName = dto.FullName, Age = dto.Age, Goal = dto.Goal, CreatedAt = DateTime.UtcNow };
        await _userRepo.AddAsync(user);
        await _userRepo.SaveChangesAsync();
        dto.Id = user.Id;
        return dto;
    }

    public async Task AddSkillSelectionsAsync(int userId, List<int> skillIds)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user != null)
        {
            // Simple logic for MVP: just clear and add
            foreach (var sid in skillIds)
            {
                user.SkillSelections.Add(new UserSkillSelection { UserId = userId, SkillId = sid });
            }
            await _userRepo.SaveChangesAsync();
        }
    }
}

public class SkillService : ISkillService
{
    private readonly ISkillRepository _skillRepo;
    public SkillService(ISkillRepository skillRepo) => _skillRepo = skillRepo;
    public async Task<IEnumerable<SkillDto>> GetAllSkillsAsync()
    {
        var skills = await _skillRepo.GetAllAsync();
        return skills.Select(s => new SkillDto { Id = s.Id, Name = s.Name, Description = s.Description, IconName = s.IconName });
    }
}

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

public class RoadmapService : IRoadmapService
{
    private readonly IRoadmapRepository _roadmapRepo;
    private readonly IAiRoadmapService _aiService;
    public RoadmapService(IRoadmapRepository roadmapRepo, IAiRoadmapService aiService)
    {
        _roadmapRepo = roadmapRepo;
        _aiService = aiService;
    }

    public async Task<RoadmapDto> GetUserRoadmapAsync(int userId)
    {
        var items = await _roadmapRepo.GetByUserIdAsync(userId);
        if (!items.Any())
        {
            var fakeRoadmap = await _aiService.GenerateRoadmapAsync(userId, new List<string> { "Communication" });
            foreach (var item in fakeRoadmap.Items)
            {
                await _roadmapRepo.AddAsync(new RoadmapItem { UserId = userId, WeekNumber = item.WeekNumber, Title = item.Title, Description = item.Description });
            }
            await _roadmapRepo.SaveChangesAsync();
            items = await _roadmapRepo.GetByUserIdAsync(userId);
        }

        return new RoadmapDto
        {
            UserId = userId,
            Items = items.Select(i => new RoadmapItemDto { Id = i.Id, WeekNumber = i.WeekNumber, Title = i.Title, Description = i.Description, IsCompleted = i.IsCompleted }).ToList()
        };
    }

    public async Task MarkCompleteAsync(int itemId)
    {
        var item = await _roadmapRepo.GetByIdAsync(itemId);
        if (item != null)
        {
            item.IsCompleted = true;
            await _roadmapRepo.SaveChangesAsync();
        }
    }
}

public class ProgressService : IProgressService
{
    private readonly IProgressRepository _progressRepo;
    public ProgressService(IProgressRepository progressRepo) => _progressRepo = progressRepo;
    public async Task<IEnumerable<ProgressDto>> GetUserProgressAsync(int userId)
    {
        var logs = await _progressRepo.GetByUserIdAsync(userId);
        return logs.Select(l => new ProgressDto { UserId = l.UserId, SkillId = l.SkillId, SkillName = l.Skill.Name, PercentComplete = l.PercentComplete, UpdatedAt = l.UpdatedAt });
    }
}

public class CaseStudyService : ICaseStudyService
{
    private readonly ICaseStudyRepository _repo;
    public CaseStudyService(ICaseStudyRepository repo) => _repo = repo;
    public async Task<IEnumerable<CaseStudyDto>> GetCaseStudiesBySkillAsync(int skillId)
    {
        var list = await _repo.GetBySkillIdAsync(skillId);
        return list.Select(cs => new CaseStudyDto { Id = cs.Id, Title = cs.Title, Scenario = cs.Scenario, SkillId = cs.SkillId });
    }
    public async Task<CaseStudyDto?> GetCaseStudyByIdAsync(int id)
    {
        var cs = await _repo.GetWithDetailsAsync(id);
        if (cs == null) return null;
        return new CaseStudyDto
        {
            Id = cs.Id,
            Title = cs.Title,
            Scenario = cs.Scenario,
            SkillId = cs.SkillId,
            Options = cs.Options.Select(o => new CaseStudyOptionDto { Id = o.Id, OptionText = o.OptionText, IsRecommended = o.IsRecommended, Feedback = o.Feedback }).ToList()
        };
    }
}

public class MentorService : IMentorService
{
    private readonly IMentorRepository _repo;
    public MentorService(IMentorRepository repo) => _repo = repo;
    public async Task<IEnumerable<MentorDto>> GetAllAsync()
    {
        var list = await _repo.GetAllAsync();
        return list.Select(m => new MentorDto { Id = m.Id, Name = m.Name, Expertise = m.Expertise, ShortBio = m.ShortBio, AvatarUrl = m.AvatarUrl });
    }
}
