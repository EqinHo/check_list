using Check_List_API.Data;
using Checklist_API.Features.JWT.Features;
using Checklist_API.Features.JWT.Features.Interfaces;
using Checklist_API.Features.Login.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Checklist_API.Features.Login.Controller;
[Route("api/v1/login")]
[ApiController]
public class LoginController(IUserAuthenticationService authService, ITokenGenerator tokenGenerator, ILogger<LoginController> logger)
                            : ControllerBase
{
    private readonly IUserAuthenticationService _authService = authService;
    private readonly ITokenGenerator _tokenGenerator = tokenGenerator;
    private readonly ILogger<LoginController> _logger = logger;

    [AllowAnonymous]
    // POST https://localhost:7070/api/v1/login
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO) // spør gpt om ok Task her
    {
        _logger.LogInformation("User logging in: {username}", loginDTO.UserName);

        var user = await _authService.AuthenticateUserAsync(loginDTO);

        if (user == null)
        {
            return Unauthorized("Not Authorized");
        }

        var tokenString = await _tokenGenerator.GenerateJSONWebTokenAsync(user);
        return Ok(new TokenResponse{ Token = tokenString });
    }
}