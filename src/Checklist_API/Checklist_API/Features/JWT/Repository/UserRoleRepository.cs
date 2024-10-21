using Check_List_API.Data;
using Checklist_API.Features.JWT.Entity;
using Checklist_API.Features.JWT.Interfaces;
using Checklist_API.Features.Users.Entity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Security;

namespace Checklist_API.Features.JWT.Repository;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly CheckListDbContext _dbContext;
    private readonly ILogger<UserRoleRepository> _logger;

    public UserRoleRepository(CheckListDbContext dbContext, ILogger<UserRoleRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IEnumerable<UserRole>> GetUserRolesAsync(UserId id)
    {
        _logger.LogInformation("Retrieving roles for user with ID: {UserId}", id);

        var roles = await _dbContext.UserRole.Where(ur => ur.UserId == id).ToListAsync();

        return roles;
    }
}
