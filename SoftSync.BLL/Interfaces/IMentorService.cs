using SoftSync.Common.Dtos;

namespace SoftSync.BLL.Interfaces;

public interface IMentorService
{
    Task<IEnumerable<MentorDto>> GetAllAsync();
    Task<MentorDto?> GetByIdAsync(int id);
}
