using SoftSync.Common.Dtos;
using SoftSync.Common.Dtos.Assessment;

namespace SoftSync.BLL.Interfaces;

// AI Interfaces (Specific names requested by user)
public interface IAiAssessmentService
{
    Task<AssessmentResultDto> EvaluateAsync(List<UserAnswerDto> answers);
}
