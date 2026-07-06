using SoftSync.BLL.Interfaces;
using SoftSync.Common.Dtos;
using SoftSync.Common.Dtos.Auth;
using SoftSync.DAL.Entities;
using SoftSync.DAL.Repositories;

namespace SoftSync.BLL.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    public UserService(IUserRepository userRepo) => _userRepo = userRepo;

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user == null) return null;
        return new UserDto { Id = user.Id, FullName = user.FullName, Age = user.Age, Goal = user.Goal, CreatedAt = user.CreatedAt };
    }

    public async Task<UserDto> CreateUserAsync(UserDto dto)
    {
        var user = new User { FullName = dto.FullName, Age = dto.Age, Goal = dto.Goal, CreatedAt = DateTime.UtcNow };
        await _userRepo.AddAsync(user);
        await _userRepo.SaveChangesAsync();
        dto.Id = user.Id;
        return dto;
    }

    public async Task AddSkillSelectionsAsync(int userId, List<int> skillIds)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user != null)
        {
            // Simple logic for MVP: just clear and add
            foreach (var sid in skillIds)
            {
                user.SkillSelections.Add(new UserSkillSelection { UserId = userId, SkillId = sid });
            }
            await _userRepo.SaveChangesAsync();
        }
    }

    public async Task<AuthResultDto> UpdateProfileAsync(int userId, string fullName, int age, string goal)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null)
            return AuthResultDto.Fail("Không tìm thấy tài khoản.");

        user.FullName = fullName;
        user.Age = age;
        user.Goal = goal;
        _userRepo.Update(user);
        await _userRepo.SaveChangesAsync();

        var dto = new UserDto { Id = user.Id, FullName = user.FullName, Email = user.Email, Age = user.Age, Role = user.Role, Goal = user.Goal, CreatedAt = user.CreatedAt };
        return AuthResultDto.Ok("Cập nhật hồ sơ thành công!", dto);
    }
}
