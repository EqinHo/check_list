using Check_List_API.Data;
using Checklist_API.Features.JWT.Features;
using Checklist_API.Features.JWT.Features.Interfaces;
using Checklist_API.Features.JWT.Interfaces;
using Checklist_API.Features.JWT.Repository;
using Checklist_API.Features.Login.DTOs;
using Checklist_API.Features.Users.Entity;
using Checklist_API.Features.Users.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Checklist.UnitTests.AuthenticationServiceTests;

public class AuthenticationServiceTests
{
    private readonly AuthenticationService _authenticationService;
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<ILogger<AuthenticationService>> _loggerMock = new();

    public AuthenticationServiceTests()
    {
        _authenticationService = new AuthenticationService(_userRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task AuthenticateUserAsync_WhenUserIsAuthenticated_ShouldReturnUser()
    {
        // Arrange
        var loginDTO = new LoginDTO { UserName = "ketilSveberg", Password = "string" };
        var expectedUser = new User
        {
            Id = UserId.NewId,
            FirstName = "Ketil",
            LastName = "Sveberg",
            Email = "ketilSveberg",
            HashedPassword = "$2a$11$J/m/v5v3hOVLKREX7jMZNO1xkMbtzU3vHf3Tm0Swc2MTszc0IpxO2",
            Salt = "$2a$11$55pfCgY8voiC1V4029QfR."

        };

        _userRepositoryMock.Setup(u => u.GetUserByEmailAsync(loginDTO.UserName)).ReturnsAsync(expectedUser);

        // Act
        var result = await _authenticationService.AuthenticateUserAsync(loginDTO);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUser.Id, result.Id);
        Assert.Equal(expectedUser.FirstName, result.FirstName);
        Assert.Equal(expectedUser.LastName, result.LastName);
        Assert.Equal(expectedUser.Email, result.Email);

        _userRepositoryMock.Verify(v => v.GetUserByEmailAsync(loginDTO.UserName), Times.Once());    
    }

    [Fact]
    public async Task AuthenticateUserAsync_WhenUserInvalidPassword_ShouldReturnNull()
    {
        // Arrange

        var loginDTO = new LoginDTO { UserName = "ketilSveberg", Password = "string" };

        var expextedUser = new User
        {
            Id = UserId.NewId,
            FirstName = "Ketil",
            LastName = "Sveberg",
            Email = "ketilSveberg",
            HashedPassword = "$2a$11$J/m/v5v3hOVLKREX7jMZNO1xkMbtzU3vHf3Tm0Swc2MTszc0IpxO22",
            Salt = "$2a$11$55pfCgY8voiC1V4029QfR."
        };

        _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(loginDTO.UserName)).ReturnsAsync(expextedUser);

        // Act

        var res = await _authenticationService.AuthenticateUserAsync(loginDTO);

        // Assert

        Assert.Null(res);

        _userRepositoryMock.Verify(x => x.GetUserByEmailAsync(loginDTO.UserName), Times.Once);
    }

    [Fact]
    public async Task AuthenticateUserAsync_WhenUserNotValid_ShouldReturnNull()
    {
        // Arrange

        var loginDTO = new LoginDTO { UserName = "ketilSveberg", Password = "string" };

        User? nullUser = null;

        _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(loginDTO.UserName)).ReturnsAsync(nullUser);

        // Act

        var res = await _authenticationService.AuthenticateUserAsync(loginDTO);

        // Assert

        Assert.Null(res);

        _userRepositoryMock.Verify(x => x.GetUserByEmailAsync(loginDTO.UserName), Times.Once);

    }
}