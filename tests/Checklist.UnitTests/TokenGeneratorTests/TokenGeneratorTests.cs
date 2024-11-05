
using Checklist_API.Features.JWT.Entity;
using Checklist_API.Features.JWT.Features;
using Checklist_API.Features.JWT.Interfaces;
using Checklist_API.Features.Users.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Checklist.UnitTests.TokenGeneratorTests;
public class TokenGeneratorTests
{
    private readonly TokenGenerator _tokenGenerator;
    private readonly Mock<IConfiguration> _configMock = new();
    private readonly Mock<IUserRoleRepository> _userRoleRepositoryMock = new();
    private readonly Mock<ILogger<TokenGenerator>> _loggerMock = new();

    public TokenGeneratorTests()
    {
        _tokenGenerator = new TokenGenerator(_configMock.Object, _userRoleRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GenerateJSONWebToken_whenOK_ShouldReturnToken()
    {
        // Arrange

        var user = new User()
        {
            Id = UserId.NewId,
            FirstName = "Ketil",
            LastName = "Sveberg",
            Email = "ketilSveberg",
            HashedPassword = "$2a$11$J/m/v5v3hOVLKREX7jMZNO1xkMbtzU3vHf3Tm0Swc2MTszc0IpxO22",
            Salt = "$2a$11$55pfCgY8voiC1V4029QfR."
        };

        List<UserRole> userRoles =
            [

                new UserRole{RoleName = "Admin"},
                new UserRole{RoleName = "User"},

            ];

        _userRoleRepositoryMock.Setup(x => x.GetUserRolesAsync(user.Id)).ReturnsAsync(userRoles);

        // Act

        _configMock.Setup(x => x["Jwt:Key"]).Returns("ThisismySecretKeyDoNotStoreHereForGodsSake");
        _configMock.Setup(x => x["Jwt:Issuer"]).Returns("Checklist_API");
        _configMock.Setup(x => x["Jwt:Audience"]).Returns("Checklist_API");

        var res = await _tokenGenerator.GenerateJSONWebTokenAsync(user);

        // Assert

        Assert.NotNull(res);
        Assert.IsType<string>(res);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(res);

        Assert.Equal(user.Id.ToString(), jwtToken.Claims.First(c => c.Type == "UserId").Value);
        Assert.Equal(user.Email, jwtToken.Claims.First(c => c.Type == "UserName").Value);
        Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Role && c.Value == "User");
    }

    [Fact]
    public async Task GenerateJSONWebTokenAsync_WhenUserIsNull_ShouldThrowNullException()
    {
        // Act and Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _tokenGenerator.GenerateJSONWebTokenAsync(null!));

    }

    [Fact]
    public async Task GenerateJSONWebTokenAsync_ShouldSetExpirationClaimCorrectly()
    {
        // Arrange

        User user = new()
        {
            Id = UserId.NewId,
            FirstName = "Ketil",
            LastName = "Sveberg",
            Email = "ketilsveberg@gmail.com",
            HashedPassword = "$2a$11$J/m/v5v3hOVLKREX7jMZNO1xkMbtzU3vHf3Tm0Swc2MTszc0IpxO2",
            Salt = "$2a$11$55pfCgY8voiC1V4029QfR."
        };

        _configMock.Setup(x => x["Jwt:Key"]).Returns("ThisismySecretKeyDoNotStoreHereForGodsSake");
        _configMock.Setup(x => x["Jwt:Issuer"]).Returns("Checklist_API");
        _configMock.Setup(x => x["Jwt:Audience"]).Returns("Checklist_API");

        // Act
        var res = await _tokenGenerator.GenerateJSONWebTokenAsync(user);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(res);
        var expirationClaim = jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Exp).Value;

        // Convert expirationClaim to DateTime and check it's set correctly
        var expirationDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expirationClaim)).DateTime;
        Assert.True(expirationDate > DateTime.UtcNow, "Token should not be expired");
    }

    [Fact]
    public async Task GenerateJSONWebTokenAsync_()
    {
        // Arrange
        User user = new()
        {
            Id = UserId.NewId,
            FirstName = "Ketil",
            LastName = "Sveberg",
            Email = "ketilsveberg@gmail.com",
            HashedPassword = "$2a$11$J/m/v5v3hOVLKREX7jMZNO1xkMbtzU3vHf3Tm0Swc2MTszc0IpxO2",
            Salt = "$2a$11$55pfCgY8voiC1V4029QfR."
        };
 
        _configMock.Setup(x => x["Jwt:Key"]).Returns("ThisismySecretKeyDoNotStoreHereForGodsSake");
        _configMock.Setup(x => x["Jwt:Issuer"]).Returns("Checklist_API");
        _configMock.Setup(x => x["Jwt:Audience"]).Returns("Checklist_API");
 
        var futureDateTime = DateTime.UtcNow.AddHours(4).AddMinutes(1);
 
        // Act
        var token = await _tokenGenerator.GenerateJSONWebTokenAsync(user);
 
        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var expirationClaim = jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Exp).Value;
        var expirationDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expirationClaim)).UtcDateTime;
 
        Assert.True(futureDateTime > expirationDate, "Token should be expired");
    }

}

