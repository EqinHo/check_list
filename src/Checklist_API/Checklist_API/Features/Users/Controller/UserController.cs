using Checklist_API.Features.Users.DTOs;
using Checklist_API.Features.Users.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

    [Authorize(Roles = "User")]
    // GET https://localhost:7070/api/v1/users?page=1&pageSize=10
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetAll(int page = 1, int pageSize = 10)
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
    [HttpGet("{id}", Name = "GetUserById")]
    public async Task<ActionResult<UserDTO>> GetUserById([FromRoute] Guid id)
    {
        _logger.LogInformation ("Retrieving User by Id:{id}", id);

        var res = await _userService.GetUserByIdAsync(id);

        return res != null ? Ok(res): NotFound($"No user with Id {id} was found");
    }

    // PUT https://localhost:7070/api/v1/users/
    [HttpPut("{id}")]
    public async Task<ActionResult<UserDTO>> UpdateUser(Guid id, [FromBody] UserUpdateDTO dto)
    {
        _logger.LogInformation("Updated user with UserID : {id} ", id);

        var res = await _userService.UpdateUserAsync(id, dto);
        return res != null ? Ok(res) : NotFound($"Could not Update User with Id {id}");
    }

    // DELETE api/<UserController>/5
    [HttpDelete("{id}", Name = "UpdateUserId")]
    public void Delete(int id)
    {

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