using Xunit; // For testing
using Microsoft.AspNetCore.Mvc; // For ActionResult types
using Moq;
using Checklist_API.Features.Login.Controller;
using Checklist_API.Features.JWT.Features;
using Microsoft.Extensions.Logging;
using Checklist_API.Features.Login.DTOs;
using Checklist_API.Features.Users.Entity;
using BCrypt.Net;
using Checklist_API.Features.JWT.Features.Interfaces; // For mocking

public class LoginControllerTests
{
    private readonly LoginController _loginController;
    private readonly Mock<IUserAuthenticationService> _authServiceMock = new();
    private readonly Mock<ITokenGenerator> _tokenGeneratorMock = new();
    private readonly Mock<ILogger<LoginController>> _loggerMock = new();

    public LoginControllerTests()
    {
        // Initialiser LoginController med mock avhengigheter
        _loginController = new LoginController(_authServiceMock.Object, _tokenGeneratorMock.Object, _loggerMock.Object);
    }

    [Fact] // Marker testmetoden for xUnit
    public async Task Login_WhenOk_ShouldReturnJwtToken()
    {
        // Arrange
        var loginDTO = new LoginDTO { UserName = "testuser", Password = "testpassword" };

        var user = new User { Id = UserId.NewId, FirstName = "testuser", HashedPassword = "testpassword" };

        var tokenString = "mockedToken"; // Opprett en mock JWT-token

        // Sett opp mock for autentisering av bruker
        _authServiceMock
            .Setup(x => x.AuthenticateUserAsync(loginDTO))
            .ReturnsAsync(user);  // Returner en mock bruker

        // Sett opp mock for generering av JWT-token
        _tokenGeneratorMock
            .Setup(x => x.GenerateJSONWebTokenAsync(user))
            .ReturnsAsync(tokenString);  // Returner en mock token

        // Act
        var result = await _loginController.Login(loginDTO);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;

        var actualResult = okResult!.Value as TokenResponse;
        Assert.NotNull(actualResult);
        Assert.Equal(tokenString, actualResult.Token);

        _authServiceMock.Verify(x => x.AuthenticateUserAsync(loginDTO), Times.Once());
        _tokenGeneratorMock.Verify(x => x.GenerateJSONWebTokenAsync(user), Times.Once());
    }

    [Fact]
    public async Task Login_WhenFail_ShouldReturnJWTToken()
    {
        // Arrange
        var InvalidLoginDTO = new LoginDTO { UserName = "testuser", Password = "testpassword" };
        User? user = null;
        TokenResponse InvalidToken = new TokenResponse { Token = "abcd" };

        _authServiceMock.Setup(X => X.AuthenticateUserAsync(InvalidLoginDTO)).ReturnsAsync(user);
        //_tokenGeneratorMock.Setup(x => x.GenerateJSONWebTokenAsync(user!)).ReturnsAsync(InvalidLoginDTO);

        // Act
        var result = await _loginController.Login(InvalidLoginDTO);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<UnauthorizedObjectResult>(result);
        var failResult = result as UnauthorizedObjectResult;
        Assert.Equal("Not Authorized", failResult!.Value);

        _authServiceMock.Verify(x => x.AuthenticateUserAsync(InvalidLoginDTO), Times.Once());
        _tokenGeneratorMock.Verify(x => x.GenerateJSONWebTokenAsync(user!), Times.Never());
    }
}
