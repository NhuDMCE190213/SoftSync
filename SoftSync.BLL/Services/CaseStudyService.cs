using SoftSync.BLL.Interfaces;
using SoftSync.Common.Dtos.CaseStudy;
using SoftSync.DAL.Repositories;

namespace SoftSync.BLL.Services;

public class CaseStudyService : ICaseStudyService
{
    private readonly ICaseStudyRepository _repo;
    public CaseStudyService(ICaseStudyRepository repo) => _repo = repo;
    public async Task<IEnumerable<CaseStudyDto>> GetCaseStudiesBySkillAsync(int skillId)
    {
        var list = await _repo.GetBySkillIdAsync(skillId);
        return list.Select(cs => new CaseStudyDto { Id = cs.Id, Title = cs.Title, Scenario = cs.Scenario, SkillId = cs.SkillId });
    }
    public async Task<CaseStudyDto?> GetCaseStudyByIdAsync(int id)
    {
        var cs = await _repo.GetWithDetailsAsync(id);
        if (cs == null) return null;
        return new CaseStudyDto
        {
            Id = cs.Id,
            Title = cs.Title,
            Scenario = cs.Scenario,
            SkillId = cs.SkillId,
            Options = cs.Options.Select(o => new CaseStudyOptionDto { Id = o.Id, OptionText = o.OptionText, IsRecommended = o.IsRecommended, Feedback = o.Feedback }).ToList()
        };
    }
}
