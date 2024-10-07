using Check_List_API.Data;
using Checklist_API.Features.JWT.Entity;
using Checklist_API.Features.Users.Entity;
using Checklist_API.Features.Users.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Checklist_API.Features.Users.Repository;

public class UserRepository : IUserRepository
{
    private readonly CheckListDbContext _dbContext;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(CheckListDbContext dbContext, ILogger<UserRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IEnumerable<User>> GetAllAsync(int page, int pageSize)
    {
        _logger.LogDebug("Getting users from db");

        int itemToSkip = (page - 1) * pageSize;

        return await _dbContext.User
            .OrderBy(x => x.Id)
            .Skip(itemToSkip)
            .Take(pageSize)
            .Distinct()
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<User?> GetByIdAsync(UserId id)
    {
        throw new NotImplementedException();
    }

    public Task<User?> UpdateAsync(UserId id, User user)
    {
        throw new NotImplementedException();
    }

    public Task<User?> DeleteAsync(UserId id)
    {
        throw new NotImplementedException();
    }

    public async Task<User?> RegisterAsync(User user)
    {
        _logger.LogDebug("Adding user: {user} to db", user.Email);

        var res = await _dbContext.User.AddAsync(user);

        JWTUserRole roleAssignment = new()
        {
            RoleName = "User",
            UserId = user.Id,            
            DateCreated = DateTime.Now,
            DateUpdated = DateTime.Now
        }; 

        await _dbContext.JWTUserRole.AddAsync(roleAssignment);
        await _dbContext.SaveChangesAsync();

        return res.Entity;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        _logger.LogDebug("Getting user by email: {email} from db", email);

        var res = await _dbContext.User.FirstOrDefaultAsync(x => x.Email.Equals(email));
        return res;
    }
}
