<<<<<<< HEAD
﻿using BCrypt.Net;
using Check_List_API.Data;
using Checklist_API.Features.Login.DTOs;
=======
﻿using Check_List_API.Data;
using Checklist_API.Features.Login.DTOs;
using Checklist_API.Features.Users.DTOs;
>>>>>>> 7dc864fa4bae6c37c62f623b39c9d8b749d98245
using Checklist_API.Features.Users.Entity;
using Microsoft.EntityFrameworkCore;

namespace Checklist_API.Features.JWT.Features;

public class AuthenticationService(CheckListDbContext dbContext, ILogger<AuthenticationService> logger)
{
    private readonly CheckListDbContext _dbContext = dbContext;
    private readonly ILogger<AuthenticationService> _logger = logger;

<<<<<<< HEAD
    public async Task<User?> AuthenticateUserAsync(LoginDTO loginDTO)
=======

    public async Task<User?> AuthenticateUserAsync(LoginDTO loginDTO) 
>>>>>>> 7dc864fa4bae6c37c62f623b39c9d8b749d98245
    {
        _logger.LogInformation("Authenticating user: {username}", loginDTO.UserName);

        var user = await _dbContext.User.FirstOrDefaultAsync(u => u.Email == loginDTO.UserName);

<<<<<<< HEAD
        if (user != null && BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.HashedPassword))
        {
            return user;
        }
        
        return null;
    }
=======
        if (user != null && BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.HashedPassword)) 
        {
            return user;
        }

        return null;
    }     
>>>>>>> 7dc864fa4bae6c37c62f623b39c9d8b749d98245
}
