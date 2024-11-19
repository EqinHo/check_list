


using Checklist_API.Features.Login.DTOs;
using Checklist_API.Features.Users.Entity;

namespace Checklist_API.Features.Users.Repository.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsersAsync(int page, int pageSize);
    Task<User?> GetUserByIdAsync(UserId id);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> UpdateUserAsync(UserId id, User user);
    Task<User?> DeleteUserAsync(UserId id);
    Task<User?> RegisterUserAsync(User user);

}
