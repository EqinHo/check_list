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

<<<<<<< HEAD
    [Authorize(Roles = "Admin")]
=======
    [Authorize(Roles = "User")]
>>>>>>> 7dc864fa4bae6c37c62f623b39c9d8b749d98245
    // GET https://localhost:7070/api/v1/users?page=1&pageSize=10
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetAll(int page = 1, int pageSize = 10)
    {
        _logger.LogInformation("Getting all Users");

        if (page < 1 || pageSize < 1 || pageSize > 50)
        {
            _logger.LogWarning("Invalid pagination parameters Page: {page}, PageSize: {pageSize}", page, pageSize);

            return BadRequest("Invalid pagination parameters - MIN page = 1, MAX pageSize = 50 ");
        }

        var res = await _userService.GetAllAsync(page, pageSize);

        return res != null ? Ok(res) : NotFound("Could not find any users");
    }

    // GET api/<UserController>/5 
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }
    // PUT api/<UserController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<UserController>/5
    [HttpDelete("{id}")]
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
