namespace SoftSync.BLL.Interfaces
{
    public interface IAnswerAnalysisService
    {
        Task<List<string>> GetRecentMistakePatternsAsync(int userId, int skillId, int take = 20);
    }
}
