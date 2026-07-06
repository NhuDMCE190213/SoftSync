using SoftSync.BLL.Interfaces;
using SoftSync.Common.Dtos;
using SoftSync.DAL.Repositories;

namespace SoftSync.BLL.Services;

public class SkillService : ISkillService
{
    private readonly ISkillRepository _skillRepo;
    public SkillService(ISkillRepository skillRepo) => _skillRepo = skillRepo;
    public async Task<IEnumerable<SkillDto>> GetAllSkillsAsync()
    {
        var skills = await _skillRepo.GetAllAsync();
        return skills.Select(s => new SkillDto { Id = s.Id, Name = s.Name, Description = s.Description, IconName = s.IconName });
    }
}
