using SoftSync.BLL.Interfaces;
using SoftSync.Common.Dtos;
using SoftSync.DAL.Repositories;

namespace SoftSync.BLL.Services
{
    public class TheoryLessonService : ITheoryLessonService
    {
        private readonly ITheoryLessonRepository _theoryLessonRepository;
        public TheoryLessonService(ITheoryLessonRepository theoryLessonRepository)
        {
            _theoryLessonRepository = theoryLessonRepository;
        }
        public async Task<List<TheoryLessonDto>> GetBySkillIdAsync(int skillId)
        {
            var lessons = await _theoryLessonRepository.GetBySkillIdAsync(skillId); 
            return lessons.Select(l => new TheoryLessonDto
            {
                Id = l.Id,
                Title = l.Title,
                ContentMarkdown = l.ContentMarkdown,
                VideoUrl = l.VideoUrl,
                OrderIndex = l.OrderIndex
            }).ToList();
        }
    }
}
