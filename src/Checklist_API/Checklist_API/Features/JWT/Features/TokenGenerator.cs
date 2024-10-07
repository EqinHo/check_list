using Check_List_API.Data;
using Checklist_API.Features.Users.Entity;
<<<<<<< HEAD
=======
using Microsoft.EntityFrameworkCore;
>>>>>>> 7dc864fa4bae6c37c62f623b39c9d8b749d98245
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Checklist_API.Features.JWT.Features;

<<<<<<< HEAD
public class TokenGenerator(IConfiguration config, CheckListDbContext dbContext, ILogger<AuthenticationService> logger)
{
    private readonly IConfiguration _config = config; // trengs for tilgang til appsettings pga JWT secretkey etc ligger der
    private readonly CheckListDbContext _dbContext = dbContext;
    private readonly ILogger<AuthenticationService> _logger = logger;

    public async Task<string> GenerateJSONWebTokenASync(User user)
=======
public class TokenGenerator(IConfiguration config, CheckListDbContext dbContext, ILogger<TokenGenerator> logger)
{
    private readonly IConfiguration _config = config; // trengs for tilgang til appsettings pga JWT secretkey etc ligger der
    private readonly CheckListDbContext _dbContext = dbContext;
    private readonly ILogger<TokenGenerator> _logger = logger;

    public async Task<string> GenerateJSONWebTokenAsync(User user) 
>>>>>>> 7dc864fa4bae6c37c62f623b39c9d8b749d98245
    {
        _logger.LogInformation("Generating token");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims = [];
<<<<<<< HEAD
        
        var userRoles = _dbContext.JWTUserRole.Where(ur => ur.UserId == user.Id);
=======
        var userRoles = await _dbContext.JWTUserRole.Where(ur => ur.UserId == user.Id).ToListAsync();
>>>>>>> 7dc864fa4bae6c37c62f623b39c9d8b749d98245

        claims.Add(new Claim("UserId", user.Id.ToString()));
        claims.Add(new Claim("UserName", user.Email.ToString()));

<<<<<<< HEAD
        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.ToString()!));
        }

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Issuer"],
=======
        foreach (var role in userRoles) 
        {
            claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
        }

        var token = new JwtSecurityToken
            (
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
>>>>>>> 7dc864fa4bae6c37c62f623b39c9d8b749d98245
            claims,
            expires: DateTime.Now.AddMinutes(240),
            signingCredentials: credentials
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
