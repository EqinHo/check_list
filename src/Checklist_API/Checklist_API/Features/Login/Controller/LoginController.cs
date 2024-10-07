using Check_List_API.Data;
using Checklist_API.Features.JWT.Features;
using Checklist_API.Features.Login.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Checklist_API.Features.Login.Controller;
[Route("api/v1/login")]
[ApiController]
<<<<<<< HEAD
public class LoginController(AuthenticationService authService, TokenGenerator tokenGenerator, ILogger<LoginController> logger) 
=======
public class LoginController(AuthenticationService authService, TokenGenerator tokenGenerator, ILogger<LoginController> logger)
>>>>>>> 7dc864fa4bae6c37c62f623b39c9d8b749d98245
                            : ControllerBase
{
    private readonly AuthenticationService _authService = authService;
    private readonly TokenGenerator _tokenGenerator = tokenGenerator;
    private readonly ILogger<LoginController> _logger = logger;

    [AllowAnonymous]
<<<<<<< HEAD
    //  POST https://localhost:7070/api/v1/users/register
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
    {
        _logger.LogInformation("User Logging in: {username}", loginDTO.UserName);
=======
    // POST https://localhost:7070/api/v1/login
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO) // spør gpt om ok Task her
    {
        _logger.LogInformation("User logging in: {username}", loginDTO.UserName);
>>>>>>> 7dc864fa4bae6c37c62f623b39c9d8b749d98245

        var user = await _authService.AuthenticateUserAsync(loginDTO);

        if (user == null)
        {
            return Unauthorized("Not Authorized");
        }

<<<<<<< HEAD
        var tokenString = await _tokenGenerator.GenerateJSONWebTokenASync(user);
=======
        var tokenString = await _tokenGenerator.GenerateJSONWebTokenAsync(user);
>>>>>>> 7dc864fa4bae6c37c62f623b39c9d8b749d98245
        return Ok(new { token = tokenString });
    }
}