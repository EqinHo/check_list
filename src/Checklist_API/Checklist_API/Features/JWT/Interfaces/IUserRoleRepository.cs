using Checklist_API.Features.JWT.Entity;
using Checklist_API.Features.Users.Entity;

namespace Checklist_API.Features.JWT.Interfaces;

public interface IUserRoleRepository
{
    Task<IEnumerable<UserRole>> GetUserRolesAsync(UserId userid);
}
