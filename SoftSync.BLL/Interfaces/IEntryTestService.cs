using SoftSync.Common.Dtos.EntryTest;

namespace SoftSync.BLL.Interfaces;

public interface IEntryTestService
{
    Task<List<EntryTestQuestionDto>> GetQuestionsAsync();
    Task<EntryTestResultDto> SubmitAsync(int userId, List<EntryTestAnswerDto> answers);
    Task<EntryTestResultDto?> GetResultAsync(int userId);
}
