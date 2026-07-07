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
    private readonly ITheoryLessonRepository _theoryLessonRepo;       // MỚI
    private readonly IMiniGameAttemptRepository _attemptRepo;         // MỚI

    public ProgressSyncService(IProgressRepository progressRepo,
                               IRoadmapRepository roadmapRepo,
                               ITheoryLessonProgressRepository theoryProgressRepo,
                               ITheoryLessonRepository theoryLessonRepo,
                               IMiniGameAttemptRepository attemptRepo,
                               IConfiguration config)
    {
        _progressRepo = progressRepo;
        _roadmapRepo = roadmapRepo;
        _theoryProgressRepo = theoryProgressRepo;
        _theoryLessonRepo = theoryLessonRepo;
        _attemptRepo = attemptRepo;
        // Cách 2: ép kiểu nullable rồi mới dùng ??
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
            // Cơ chế 1: cộng % Progress
            var log = await _progressRepo.GetOrCreateAsync(userId, skillId);
            log.PercentComplete = Math.Min(100, log.PercentComplete + 10);
            log.UpdatedAt = DateTime.UtcNow;

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

        var log = await _progressRepo.GetOrCreateAsync(userId, skillId);
        log.PercentComplete = Math.Min(100, log.PercentComplete + 5); // bước nhỏ hơn mini-game vì chỉ là đọc lý thuyết
        log.UpdatedAt = DateTime.UtcNow;

        await _theoryProgressRepo.SaveChangesAsync();
        await _progressRepo.SaveChangesAsync();

        await TryCompleteRoadmapItemAsync(userId, skillId);
    }
    private async Task TryCompleteRoadmapItemAsync(int userId, int skillId)
    {
        // Điều kiện A: đã đọc hết TẤT CẢ bài lý thuyết của skill này
        var lessons = await _theoryLessonRepo.GetBySkillIdAsync(skillId);
        var lessonIds = lessons.Select(l => l.Id).ToList();
        bool theoryDone = lessonIds.Count == 0
            || await _theoryProgressRepo.CountCompletedAsync(userId, lessonIds) >= lessonIds.Count;

        // Điều kiện B: có ít nhất 1 lần chơi mini-game ĐẠT ngưỡng của skill này
        var attempts = await _attemptRepo.GetRecentByUserAndSkillAsync(userId, skillId, 20);
        bool gameDone = attempts.Any(a =>
            a.MaxScore > 0 &&
            (double)a.TotalScore / a.MaxScore >= _passingRatio);

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