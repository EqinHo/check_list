using Checklist_API.Features.Users.DTOs;

namespace Checklist_API.Features.Users.Service.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDTO>> GetAllAsync(int page, int pageSize);
    Task<UserDTO?> GetByIdAsync(Guid userId);
    Task<UserDTO?> UpdateAsync(Guid id, UserDTO dto);
    Task<UserDTO?> DeleteAsync(Guid id);
    Task<UserDTO?> RegisterUserAsync(UserRegistrationDTO dto);
}
