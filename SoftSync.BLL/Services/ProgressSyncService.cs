using Microsoft.Extensions.Configuration;
using SoftSync.BLL.Interfaces;
using SoftSync.DAL.Entities;
using SoftSync.DAL.Repositories;

namespace SoftSync.BLL.Services;

public class ProgressSyncService : IProgressSyncService
{
    private readonly double _passingRatio;
    private readonly double _remedialRatio;
    private readonly IProgressRepository _progressRepo;
    private readonly IRoadmapRepository _roadmapRepo;
    private readonly ITheoryLessonProgressRepository _theoryProgressRepo;
    private readonly ITheoryLessonRepository _theoryLessonRepo;
    private readonly IMiniGameRepository _miniGameRepo;             // MỚI - cần để biết TỔNG số minigame của skill
    private readonly IMiniGameAttemptRepository _attemptRepo;

    public ProgressSyncService(IProgressRepository progressRepo,
                               IRoadmapRepository roadmapRepo,
                               ITheoryLessonProgressRepository theoryProgressRepo,
                               ITheoryLessonRepository theoryLessonRepo,
                               IMiniGameRepository miniGameRepo,     // MỚI
                               IMiniGameAttemptRepository attemptRepo,
                               IConfiguration config)
    {
        _progressRepo = progressRepo;
        _roadmapRepo = roadmapRepo;
        _theoryProgressRepo = theoryProgressRepo;
        _theoryLessonRepo = theoryLessonRepo;
        _miniGameRepo = miniGameRepo;
        _attemptRepo = attemptRepo;
        _passingRatio = config.GetValue<double?>("Gamification:PassingScoreThreshold") ?? 0.7;
        _remedialRatio = config.GetValue<double?>("Gamification:RemedialScoreThreshold") ?? 0.5;
    }

    public async Task SyncFromMiniGameAsync(int userId, int skillId, int miniGameId, int totalScore, int maxScore)
    {
        if (maxScore <= 0) return;
        double ratio = (double)totalScore / maxScore;
        bool passed = ratio >= _passingRatio;

        var items = (await _roadmapRepo.GetByUserIdAsync(userId)).ToList();

        if (passed)
        {
            // Cơ chế 1: tính lại % dựa trên tỉ lệ nội dung đã hoàn thành (không cộng cứng nữa)
            await RecalculatePercentAsync(userId, skillId);

            await TryCompleteRoadmapItemAsync(userId, skillId);
        }
        else if (ratio < _remedialRatio)
        {
            // Cơ chế 2: chèn RoadmapItem ôn tập động cho tuần kế tiếp
            bool hasPendingRemedial = items.Any(i => i.SkillId == skillId && i.IsRemedial && !i.IsCompleted);
            if (!hasPendingRemedial)
            {
                int nextWeek = (items.Any() ? items.Max(i => i.WeekNumber) : 0) + 1;
                await _roadmapRepo.AddAsync(new RoadmapItem
                {
                    UserId = userId,
                    SkillId = skillId,
                    WeekNumber = nextWeek,
                    Title = "Ôn tập kỹ năng",
                    Description = "Bạn cần ôn lại nội dung này trước khi tiếp tục.",
                    MiniGameId = miniGameId,
                    IsRemedial = true
                });
            }
        }

        await _progressRepo.SaveChangesAsync();
        await _roadmapRepo.SaveChangesAsync();
    }

    public async Task SyncFromTheoryLessonAsync(int userId, int skillId, int theoryLessonId)
    {
        // tránh cộng % trùng nếu user bấm qua lại bài đã học
        bool already = await _theoryProgressRepo.ExistsAsync(userId, theoryLessonId);
        if (already) return;

        await _theoryProgressRepo.AddAsync(new TheoryLessonProgress
        {
            UserId = userId,
            TheoryLessonId = theoryLessonId,
            CompletedAt = DateTime.UtcNow
        });
        await _theoryProgressRepo.SaveChangesAsync();

        // tính lại % dựa trên tỉ lệ nội dung đã hoàn thành (không cộng cứng +5 nữa)
        await RecalculatePercentAsync(userId, skillId);
        await _progressRepo.SaveChangesAsync();

        await TryCompleteRoadmapItemAsync(userId, skillId);
    }

    /// <summary>
    /// Tính lại PercentComplete của 1 skill dựa trên tỉ lệ:
    /// (số bài lý thuyết đã học + số minigame đã pass ít nhất 1 lần) / (tổng bài lý thuyết + tổng minigame của skill).
    /// Thay cho việc cộng cứng +5 / +10 trước đây (nguyên nhân khiến % không bao giờ đạt 100
    /// vì tổng các bước cộng cứng không khớp với tổng số nội dung thật của từng skill).
    /// </summary>
    private async Task RecalculatePercentAsync(int userId, int skillId)
    {
        var lessons = await _theoryLessonRepo.GetBySkillIdAsync(skillId);
        var lessonIds = lessons.Select(l => l.Id).ToList();
        int completedLessons = lessonIds.Count == 0
            ? 0
            : await _theoryProgressRepo.CountCompletedAsync(userId, lessonIds);

        var games = await _miniGameRepo.GetBySkillIdAsync(skillId);
        int totalGames = games.Count;
        int completedGames = totalGames == 0
            ? 0
            : await _attemptRepo.CountDistinctPassedAsync(userId, skillId, _passingRatio);

        int totalItems = lessonIds.Count + totalGames;

        var log = await _progressRepo.GetOrCreateAsync(userId, skillId);
        log.PercentComplete = totalItems == 0
            ? 0
            : (int)Math.Round(100.0 * (completedLessons + completedGames) / totalItems);
        log.UpdatedAt = DateTime.UtcNow;
    }

    private async Task TryCompleteRoadmapItemAsync(int userId, int skillId)
    {
        // Điều kiện A: đã đọc hết TẤT CẢ bài lý thuyết của skill này
        var lessons = await _theoryLessonRepo.GetBySkillIdAsync(skillId);
        var lessonIds = lessons.Select(l => l.Id).ToList();
        bool theoryDone = lessonIds.Count == 0
            || await _theoryProgressRepo.CountCompletedAsync(userId, lessonIds) >= lessonIds.Count;

        // Điều kiện B: đã pass TẤT CẢ minigame của skill này (không chỉ 1 game bất kỳ trong 20 lượt gần nhất)
        var games = await _miniGameRepo.GetBySkillIdAsync(skillId);
        bool gameDone = games.Count == 0
            || await _attemptRepo.CountDistinctPassedAsync(userId, skillId, _passingRatio) >= games.Count;

        if (!theoryDone || !gameDone) return;

        // Tick đúng tuần roadmap gắn skill này mà chưa hoàn thành
        var items = (await _roadmapRepo.GetByUserIdAsync(userId)).ToList();
        var target = items.FirstOrDefault(i => i.SkillId == skillId && !i.IsCompleted);
        if (target != null)
        {
            target.IsCompleted = true;
            _roadmapRepo.Update(target);
            await _roadmapRepo.SaveChangesAsync();
        }
    }
}