using SoftSync.Common.Dtos.MiniGame;

namespace SoftSync.BLL.Interfaces;

public interface IMiniGameService
{
    Task<List<MiniGameDto>> GetMiniGamesBySkillIdAsync(int skillId);
    Task<List<MiniGameQuestionDto>> GetRandomQuestionsAsync(int miniGameId);
    Task<int> SubmitAttemptAsync(int userId, int miniGameId, List<MiniGameAnswerDto> answers);
    Task<int> GetByIdAsync(int miniGameId);
}
