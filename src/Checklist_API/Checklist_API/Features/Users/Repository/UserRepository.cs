using Check_List_API.Data;
using Checklist_API.Features.JWT.Entity;
using Checklist_API.Features.Login.DTOs;
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

    public async Task<IEnumerable<User>> GetAllUsersAsync(int page, int pageSize)
    {
        _logger.LogInformation("Retrieving users from db");

        int itemToSkip = (page - 1) * pageSize;

        return await _dbContext.User
            .OrderBy(x => x.LastName)
            .Skip(itemToSkip)
            .Take(pageSize)
            .Distinct()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(UserId id)
    {
        _logger.LogInformation("Retrieving user by Id : {id}", id);

        return await _dbContext.User.FirstOrDefaultAsync(x=> x.Id == id);
    }

    public async Task<User?> UpdateUserAsync(UserId id, User user)
    {
        _logger.LogInformation("Updating user with Id : {id}", id);

        var usr = await _dbContext.User.FindAsync(id);
        if (usr == null) 
        { 
            return null;
        }

        usr.FirstName = string.IsNullOrWhiteSpace(user.FirstName) ? user.FirstName : user.FirstName;
        usr.LastName = string.IsNullOrWhiteSpace(user.LastName) ? user.LastName : user.LastName;
        usr.Email = string.IsNullOrWhiteSpace(user.Email) ? user.Email : user.Email;
        usr.PhoneNumber = string.IsNullOrWhiteSpace(user.PhoneNumber) ? user.PhoneNumber : user.PhoneNumber;
        usr.DateUpdated = DateTime.Now;

        await _dbContext.SaveChangesAsync();
        return usr;
    }

    public async Task<User?> DeleteUserAsync(UserId id)
    {
        _logger.LogInformation("delete user with Id : {id}", id);

        var user = await _dbContext.User.FindAsync(id);

        if(user == null)
        {
            return null;
        }

        _dbContext.User.Remove(user);
        await _dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<User?> RegisterUserAsync(User user)
    {
        _logger.LogDebug("Adding user: {user} to db", user.Email);

        var res = await _dbContext.User.AddAsync(user);

        UserRole roleAssignment = new()
        {
            RoleName = "User",
            UserId = user.Id,            
            DateCreated = DateTime.Now,
            DateUpdated = DateTime.Now
        }; 

        await _dbContext.UserRole.AddAsync(roleAssignment);
        await _dbContext.SaveChangesAsync();

        return res.Entity;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        _logger.LogDebug("Retrieving user by email: {email} from db", email);

        var res = await _dbContext.User.FirstOrDefaultAsync(x => x.Email.Equals(email));
        return res;
    }

}
