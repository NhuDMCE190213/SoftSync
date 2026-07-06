using SoftSync.Common.Dtos.CaseStudy;

namespace SoftSync.BLL.Interfaces;

public interface ICaseStudyService
{
    Task<IEnumerable<CaseStudyDto>> GetCaseStudiesBySkillAsync(int skillId);
    Task<CaseStudyDto?> GetCaseStudyByIdAsync(int id);
}
