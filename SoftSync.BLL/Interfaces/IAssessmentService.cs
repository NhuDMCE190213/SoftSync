using SoftSync.Common.Dtos;
using SoftSync.Common.Dtos.Assessment;

namespace SoftSync.BLL.Interfaces;

public interface IAssessmentService
{
    Task<IEnumerable<AssessmentQuestionDto>> GetAssessmentQuestionsAsync(int userId);
    Task SubmitAssessmentAsync(int userId, List<UserAnswerDto> answers);
    Task<IEnumerable<AssessmentResultDto>> GetLatestResultsAsync(int userId);
}
