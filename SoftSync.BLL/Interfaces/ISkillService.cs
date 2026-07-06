using SoftSync.Common.Dtos;

namespace SoftSync.BLL.Interfaces;

public interface ISkillService
{
    Task<IEnumerable<SkillDto>> GetAllSkillsAsync();
}
