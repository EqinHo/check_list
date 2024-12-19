using Check_List_API.Data;
using Checklist_API.Features.JWT.Features.Interfaces;
using Checklist_API.Features.JWT.Interfaces;
using Checklist_API.Features.Users.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Checklist_API.Features.JWT.Features;

public class TokenGenerator(IConfiguration config, IUserRoleRepository userRoleRepository, ILogger<TokenGenerator> logger) : ITokenGenerator
{
    private readonly IConfiguration _config = config; // trengs for tilgang til appsettings pga JWT secretkey etc ligger der
    private readonly IUserRoleRepository _userRoleRepository = userRoleRepository;
    private readonly ILogger<TokenGenerator> _logger = logger;

    public async Task<string> GenerateJSONWebTokenAsync(User user)
    {
        _logger.LogInformation("Generating token");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var userRoles = await _userRoleRepository.GetUserRolesAsync(user.Id);

        List<Claim> claims = [];
        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.userId.ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Name, user.Email.ToString()));

        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
        }

        var token = new JwtSecurityToken
            (
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(240),
            signingCredentials: credentials
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}