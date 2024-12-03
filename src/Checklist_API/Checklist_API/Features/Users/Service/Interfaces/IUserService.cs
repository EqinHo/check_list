using Checklist_API.Features.Users.DTOs;

namespace Checklist_API.Features.Users.Service.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDTO>> GetAllUsersAsync(int page, int pageSize);
    Task<UserDTO?> GetUserByIdAsync(Guid userId);
    Task<UserDTO?> UpdateUserAsync(Guid id, UserUpdateDTO dto);
    Task<UserDTO?> DeleteUserAsync(Guid id);
    Task<UserDTO?> RegisterUserAsync(UserRegistrationDTO dto);
}
