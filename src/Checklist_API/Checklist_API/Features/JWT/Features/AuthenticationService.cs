using Check_List_API.Data;
using Checklist_API.Features.JWT.Features.Interfaces;
using Checklist_API.Features.Login.DTOs;
using Checklist_API.Features.Users.Entity;
using Microsoft.EntityFrameworkCore;

namespace Checklist_API.Features.JWT.Features;

public class AuthenticationService(CheckListDbContext dbContext, ILogger<AuthenticationService> logger) : IUserAuthenticationService
{
    private readonly CheckListDbContext _dbContext = dbContext;
    private readonly ILogger<AuthenticationService> _logger = logger;

    public async Task<User?> AuthenticateUserAsync(LoginDTO loginDTO)
    {
        _logger.LogInformation("Authenticating user: {username}", loginDTO.UserName);

        var user = await _dbContext.User.FirstOrDefaultAsync(u => u.Email == loginDTO.UserName);

        if (user != null && BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.HashedPassword))
        {
            return user;
        }

        return null;
    }
}