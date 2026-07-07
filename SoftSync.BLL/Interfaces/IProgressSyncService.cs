namespace SoftSync.BLL.Interfaces;

public interface IProgressSyncService
{
    Task SyncFromMiniGameAsync(int userId, int skillId, int miniGameId, int totalScore, int maxScore);
    Task SyncFromTheoryLessonAsync(int userId, int skillId, int theoryLessonId); // MỚI
}