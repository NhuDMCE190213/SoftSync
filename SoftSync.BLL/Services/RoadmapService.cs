using SoftSync.BLL.Interfaces;
using SoftSync.Common.Dtos.Roadmap;
using SoftSync.Common.Enums;
using SoftSync.DAL.Entities;
using SoftSync.DAL.Repositories;

namespace SoftSync.BLL.Services;

public class RoadmapService : IRoadmapService
{
    private readonly IRoadmapRepository _roadmapRepo;
    private readonly IAiRoadmapService _aiService;
    private readonly IUserRepository _userRepo;
    private readonly IEntryTestRepository _entryTestRepo;
    private readonly ISkillRepository _skillRepo;
    private readonly IAnswerAnalysisService _answerAnalysisService;

    public RoadmapService(
        IRoadmapRepository roadmapRepo,
        IAiRoadmapService aiService,
        IUserRepository userRepo,
        IEntryTestRepository entryTestRepo,
        ISkillRepository skillRepo,
        IAnswerAnalysisService answerAnalysisService)
    {
        _roadmapRepo = roadmapRepo;
        _aiService = aiService;
        _userRepo = userRepo;
        _entryTestRepo = entryTestRepo;
        _answerAnalysisService = answerAnalysisService;
        _skillRepo = skillRepo;
    }

    public async Task<RoadmapDto> GetUserRoadmapAsync(int userId)
    {
        var items = await _roadmapRepo.GetByUserIdAsync(userId);
        var itemList = items.ToList();
        var weekGroups = itemList.GroupBy(i => i.WeekNumber).OrderBy(g => g.Key).ToList();

        var lockedWeeks = new HashSet<int>();
        for (int idx = 1; idx < weekGroups.Count; idx++)
        {
            var prevWeek = weekGroups[idx - 1];
            bool prevWeekDone = prevWeek.All(i => i.IsCompleted);
            if (!prevWeekDone) lockedWeeks.Add(weekGroups[idx].Key);
        }
        // Nếu 1 tuần bị khoá thì mọi tuần sau nó cũng khoá theo (khoá dây chuyền)
        int? firstLockedWeek = weekGroups.Skip(1)
            .FirstOrDefault(g => lockedWeeks.Contains(g.Key))?.Key;
        if (firstLockedWeek.HasValue)
        {
            foreach (var g in weekGroups.Where(g => g.Key >= firstLockedWeek.Value))
                lockedWeeks.Add(g.Key);
        }

        return new RoadmapDto
        {
            UserId = userId,
            Items = itemList.Select(i => new RoadmapItemDto
            {
                Id = i.Id,
                SkillId = i.SkillId,
                WeekNumber = i.WeekNumber,
                Title = i.Title,
                Description = i.Description,
                IsCompleted = i.IsCompleted,
                IsLocked = lockedWeeks.Contains(i.WeekNumber)
            }).ToList()
        };
    }

    public async Task GenerateRoadmapAsync(int userId)
    {
        var existingItems = (await _roadmapRepo.GetByUserIdAsync(userId)).ToList();
        var existingSkillIds = existingItems.Where(i => i.SkillId.HasValue).Select(i => i.SkillId!.Value).ToHashSet();

        var weaknesses = await BuildWeaknessListAsync(userId);
        var newWeaknesses = weaknesses.Where(w => !existingSkillIds.Contains(w.SkillId)).ToList();
        if (!newWeaknesses.Any()) return; // không có skill mới -> không đụng vào roadmap cũ

        int nextWeek = existingItems.Any() ? existingItems.Max(i => i.WeekNumber) + 1 : 1;

        if (!existingItems.Any()) // lần đầu tiên mới cần tuần mở đầu
        {
            await _roadmapRepo.AddAsync(new RoadmapItem
            {
                UserId = userId,
                SkillId = null,
                WeekNumber = nextWeek++,
                Title = "Khám phá bản thân",
                Description = "Hoàn thành bài đánh giá năng lực và xác định mục tiêu.",
                IsCompleted = true
            });
        }

        foreach (var w in newWeaknesses.OrderBy(w => w.Level))
        {
            await _roadmapRepo.AddAsync(new RoadmapItem
            {
                UserId = userId,
                SkillId = w.SkillId,
                WeekNumber = nextWeek++,
                Title = $"Kỹ năng {w.SkillName}",
                Description = $"Cải thiện {w.SkillName} (mức hiện tại: {w.Level}).",
                IsCompleted = false
            });
        }

        await _roadmapRepo.SaveChangesAsync();
    }

    private async Task<List<SkillWeaknessDto>> BuildWeaknessListAsync(int userId)
    {
        var selectedSkillIds = await _userRepo.GetSelectedSkillIdsAsync(userId);
        var latestResult = await _entryTestRepo.GetLatestResultAsync(userId);
        var allSkills = (await _skillRepo.GetAllAsync()).ToDictionary(s => s.Id, s => s.Name);

        var pillarScores = latestResult == null
            ? new Dictionary<int, (int Score, AssessmentLevel Level)>()
            : new Dictionary<int, (int Score, AssessmentLevel Level)>
            {
                [1] = (latestResult.TimeManagementScore, latestResult.TimeManagementLevel),
                [2] = (latestResult.CommunicationScore, latestResult.CommunicationLevel),
                [3] = (latestResult.CriticalThinkingScore, latestResult.CriticalThinkingLevel)
            };

        var targetSkillIds = selectedSkillIds.Any() ? selectedSkillIds : pillarScores.Keys.ToList();

        // trong BuildWeaknessListAsync, sau khi tạo targetSkillIds
        var weaknesses = new List<SkillWeaknessDto>();
        foreach (var skillId in targetSkillIds)
        {
            var patterns = await _answerAnalysisService.GetRecentMistakePatternsAsync(userId, skillId);
            weaknesses.Add(new SkillWeaknessDto
            {
                SkillId = skillId,
                SkillName = allSkills.TryGetValue(skillId, out var name) ? name : $"Skill {skillId}",
                Score = pillarScores.TryGetValue(skillId, out var p) ? p.Score : 0,
                Level = pillarScores.TryGetValue(skillId, out var p2) ? p2.Level : AssessmentLevel.Developing,
                MistakePatterns = patterns
            });
        }
        return weaknesses;
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