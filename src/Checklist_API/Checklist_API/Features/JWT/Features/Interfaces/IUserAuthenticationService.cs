using Checklist_API.Features.Login.DTOs;
using Checklist_API.Features.Users.Entity;

namespace Checklist_API.Features.JWT.Features.Interfaces;

public interface IUserAuthenticationService
{
    Task<User?> AuthenticateUserAsync(LoginDTO loginDTO);
}
