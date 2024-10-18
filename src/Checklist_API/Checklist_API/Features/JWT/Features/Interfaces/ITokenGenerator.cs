using Checklist_API.Features.Users.Entity;

namespace Checklist_API.Features.JWT.Features.Interfaces;

public interface ITokenGenerator
{
    Task<string> GenerateJSONWebTokenAsync(User user);
}
