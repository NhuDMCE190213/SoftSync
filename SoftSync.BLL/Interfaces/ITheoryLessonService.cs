using SoftSync.Common.Dtos;

namespace SoftSync.BLL.Interfaces
{
    public interface ITheoryLessonService
    {
        Task<List<TheoryLessonDto>> GetBySkillIdAsync(int skillId);
    }
}
