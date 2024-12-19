using Checklist_API.Features.Users.DTOs;
using Checklist_API.Features.Users.Entity;
using Checklist_API.Features.Users.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Checklist_API.Features.Users.Controller;
[Route("api/v1/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    // GET https://localhost:7070/api/v1/users?page=1&pageSize=10

    [Authorize(Roles = "Admin")]
    [HttpGet(Name = "GetAllUsers")]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers(int page = 1, int pageSize = 10)
    {
        _logger.LogInformation("Retrieving all Users");


        if (page < 1 || pageSize < 1 || pageSize > 50)
        {
            _logger.LogWarning("Invalid pagination parameters Page: {page}, PageSize: {pageSize}", page, pageSize);

            return BadRequest("Invalid pagination parameters - MIN page = 1, MAX pageSize = 50 ");
        }

        var res = await _userService.GetAllUsersAsync(page, pageSize);

        return res != null ? Ok(res) : NotFound("Could not find any users");
    }

    // GET https://localhost:7070/api/v1/users
    [Authorize(Roles = "User")]
    [HttpGet("{id}", Name = "GetUserById")]
    public async Task<ActionResult<UserDTO>> GetUserById([FromRoute] Guid id)
    {
        _logger.LogInformation ("Retrieving User by Id:{id}", id);

        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

        if (!roles.Contains("Admin") && userId != id.ToString()) 
        { 
            return Unauthorized("Not authorized to get this user");
        }

        var res = await _userService.GetUserByIdAsync(id);

        return res != null ? Ok(res): NotFound($"No user with Id {id} was found");
    }

    // PUT https://localhost:7070/api/v1/users/
    [Authorize(Roles = "User")]
    [HttpPut("{id}", Name = "UpdateUserById")]
    public async Task<ActionResult<UserDTO>> UpdateUserById(Guid id, [FromBody] UserUpdateDTO dto)
    {
        _logger.LogInformation("Updated user with User ID : {id} ", id);

        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (!roles.Contains("Admin") && userId != id.ToString())
        {
            return Unauthorized("Not authorized to update this user"); 
        }

        var res = await _userService.UpdateUserAsync(id, dto);
        return res != null ? Ok(res) : NotFound($"Could not Update User with Id {id}");
    }

    // DELETE https://localhost:7070/api/v1/users/
    [HttpDelete("{id}", Name = "DeleteUserById")]
    public async Task<ActionResult<UserDTO>> DeleteUserById(Guid id)
    {
        _logger.LogInformation("Deleting User with User ID : {id}", id);

        var res = await _userService.DeleteUserAsync(id);
        return res != null ? Ok(res) : NotFound($"Could not Delete User with Id {id}");
    }

    // POST https://localhost:7070/api/v1/users/register
    [HttpPost("register", Name = "RegisterUser")]
    public async Task<ActionResult<UserDTO>> RegisterUser([FromBody] UserRegistrationDTO dto)
    {
        _logger.LogDebug("Registering new user: {email}", dto.Email);

        var res = await _userService.RegisterUserAsync(dto);
        return res != null ? Ok(res) : BadRequest("Could not register new user");
    }
}