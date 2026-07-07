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
        return new UserDto {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Age = user.Age,
            Goal = user.Goal,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<List<int>> AddSkillSelectionsAsync(int userId, List<int> skillIds)
    => await _userRepo.AddNewSkillSelectionsAsync(userId, skillIds);

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
    public async Task<List<int>> GetSelectedSkillIdsAsync(int userId)
    {
        return await _userRepo.GetSelectedSkillIdsAsync(userId); // chỉ forward xuống repo có sẵn
    }
}
