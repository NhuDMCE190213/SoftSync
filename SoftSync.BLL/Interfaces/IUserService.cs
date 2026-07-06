using SoftSync.Common.Dtos;
using SoftSync.Common.Dtos.Auth;

namespace SoftSync.BLL.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<UserDto> CreateUserAsync(UserDto userDto);
    Task AddSkillSelectionsAsync(int userId, List<int> skillIds);
    Task<AuthResultDto> UpdateProfileAsync(int userId, string fullName, int age, string goal); // MỚI
}
