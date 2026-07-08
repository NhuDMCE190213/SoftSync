using SoftSync.BLL.Interfaces;
using SoftSync.Common.Dtos;
using SoftSync.DAL.Repositories;

namespace SoftSync.BLL.Services;

public class MentorService : IMentorService
{
    private readonly IMentorRepository _repo;
    public MentorService(IMentorRepository repo) => _repo = repo;

    public async Task<IEnumerable<MentorDto>> GetAllAsync()
    {
        var list = await _repo.GetAllAsync();
        return list.Select(m => new MentorDto { Id = m.Id, Name = m.Name, Expertise = m.Expertise, ShortBio = m.ShortBio, AvatarUrl = m.AvatarUrl });
    }

    public async Task<MentorDto?> GetByIdAsync(int id)
    {
        var mentor = await _repo.GetByIdAsync(id);
        if (mentor == null) return null;
        return new MentorDto { Id = mentor.Id, Name = mentor.Name, Expertise = mentor.Expertise, ShortBio = mentor.ShortBio, AvatarUrl = mentor.AvatarUrl };
    }
}
