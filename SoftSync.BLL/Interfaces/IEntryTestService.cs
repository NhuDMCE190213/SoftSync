using SoftSync.Common.Dtos.EntryTest;

namespace SoftSync.BLL.Interfaces;

public interface IEntryTestService
{
    Task<EntryTestResultDto> SubmitAsync(int userId, List<EntryTestAnswerDto> answers);
    Task<EntryTestResultDto?> GetResultAsync(int userId);
    Task<List<EntryTestQuestionDto>> GetQuestionsAsync(int userId, List<int>? skillIdsOverride = null);
}
