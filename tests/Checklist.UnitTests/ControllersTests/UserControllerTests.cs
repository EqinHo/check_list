using Checklist_API.Features.Users.Controller;
using Checklist_API.Features.Users.DTOs;
using Checklist_API.Features.Users.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Checklist.UnitTests.ControllersTests;
public class UserControllerTests
{
    private readonly UserController _userController;
    private readonly Mock<ClaimsPrincipal> _mockUserClaim;
    private readonly Mock<IUserService> _userServiceMock = new Mock<IUserService>();
    private readonly Mock<ILogger<UserController>> _loggerMock = new Mock<ILogger<UserController>>();

    public UserControllerTests()
    {
        _mockUserClaim = new Mock<ClaimsPrincipal>();
        _mockUserClaim.Setup(u => u.Claims).Returns(new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, "fakeUserId123")
        });

        var controllercontext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = _mockUserClaim.Object }
        };

        _userController = new(_userServiceMock.Object, _loggerMock.Object)
        {
            ControllerContext = controllercontext
        };
    }

    #region GetAllUsersTests

    [Theory]
    [InlineData(1, 10)]
    [InlineData(1, 5)]

    public async Task GetAllUsersAsync_ShouldReturnAllUsers_WithPagingValues(int page, int pageSize)
    {
        // arrange

        List<UserDTO> dtos = new()
        {
            new UserDTO("Ketil", "Sveberg", "12345678", "Sveberg@.gmail.com", new DateTime(2024, 10, 17, 02, 50, 00), new DateTime(2024, 10, 17, 02, 52, 30)),
            new UserDTO("Quyen", "Ho", "23456789", "quyen99@gmail.com", new DateTime(2024, 10, 18, 02, 51, 00), new DateTime(2024, 10, 17, 03, 55, 40)),
            new UserDTO("Nico", "Ho", "12345678", "nico@gmail.com", new DateTime(2024, 10, 19, 02, 52, 00), new DateTime(2024, 10, 17, 12, 00, 45))
        };

        _userServiceMock.Setup(x => x.GetAllUsersAsync(page, pageSize)).ReturnsAsync(dtos);

        // Act

        var res = await _userController.GetAllUsers(page, pageSize);

        // Assert

        // disse unpacker res:
        var actionResult = Assert.IsType<ActionResult<IEnumerable<UserDTO>>>(res); // This checks that res is of type ActionResult<IEnumerable<UserDTO>>.
        var returnValue = Assert.IsType<OkObjectResult>(actionResult.Result); // This asserts that the Result inside actionResult is of type OkObjectResult.
        var dtoCollection = Assert.IsType<List<UserDTO>>(returnValue.Value); // This asserts that the Value inside the OkObjectResult is a List<UserDTO>, AND IT CONTAINS ALL THE DATA.

        Assert.Equal(dtos.Count, dtoCollection.Count);

        foreach (var (expected, actual) in dtos.Zip(dtoCollection, (expected, actual) => (expected, actual)))
        {
            Assert.Equal(expected.FirstName, actual.FirstName);
            Assert.Equal(expected.LastName, actual.LastName);
            Assert.Equal(expected.PhoneNumber, actual.PhoneNumber);
            Assert.Equal(expected.Email, actual.Email);
            Assert.Equal(expected.DateCreated, actual.DateCreated);
            Assert.Equal(expected.DateUpdated, actual.DateUpdated);
        }
    }


    [Theory]
    [InlineData(1, 10)]
    [InlineData(1, 5)]

    public async Task GetAllUsersAsync_ShouldReturnNotFound_AllUsers_WithPagingValues(int page, int pageSize)
    {
        // arrange

        _userServiceMock.Setup(x => x.GetAllUsersAsync(page, pageSize)).ReturnsAsync(() => null!);

        // Act

        var res = await _userController.GetAllUsers(page, pageSize);

        // Assert

        // disse unpacker res:
        var actionResult = Assert.IsType<ActionResult<IEnumerable<UserDTO>>>(res); // This checks that res is of type ActionResult<IEnumerable<UserDTO>>.
        var returnValue = Assert.IsType<NotFoundObjectResult>(actionResult.Result); // This asserts that the Result inside actionResult is of type NotFoundObjectResult.
        var errorMessage = Assert.IsType<string>(returnValue.Value); // This asserts that the Value inside the OkObjectResult is a List<UserDTO>, AND IT CONTAINS ALL THE DATA.
        Assert.Equal("Could not find any users", errorMessage);
    }

    #endregion GetAllUsersTests

    #region RegisterUserTests    

    #region using TheoryData V1

    //Use approach with TheoryData if:

    //The expected UserDTO is always a direct transformation of the input.
    //You want a more concise and easily maintainable test.

    public static TheoryData<UserRegistrationDTO, UserDTO> GetUserRegistrationDTOsWithExpectedResults()
    {
        var testData = new TheoryData<UserRegistrationDTO, UserDTO>
        {
            {
                new UserRegistrationDTO("Ketil", "Sveberg", "12345678", "Sveberg@gmail.com", "password"),
                new UserDTO("Ketil", "Sveberg", "12345678", "Sveberg@gmail.com", DateTime.UtcNow, DateTime.UtcNow)
            },
            {
                new UserRegistrationDTO("Quyen", "Ho", "42534253", "Quyen99@gmail.com", "password2"),
                new UserDTO("Quyen", "Ho", "42534253", "Quyen99@gmail.com", DateTime.UtcNow, DateTime.UtcNow)
            },
            {
                new UserRegistrationDTO("Nico", "Ho", "42534253", "Nico@gmail.com", "password3"),
                new UserDTO("Nico", "Ho", "42534253", "Nico@gmail.com", DateTime.UtcNow, DateTime.UtcNow)
            }
        };

        return testData;
    }

    [Theory]
    [MemberData(nameof(GetUserRegistrationDTOsWithExpectedResults))] // kunne brukt ClassDtaa or InlineData for p ikke ha warning, lar være her pga DTOs er serializable

    public async Task RegisterUser_WhenOK_ShouldReturnUserDTO(UserRegistrationDTO dto, UserDTO expectedUserDTO)
    {
        // Arrange
        _userServiceMock.Setup(x => x.RegisterUserAsync(dto)).ReturnsAsync(expectedUserDTO);

        // Act
        var result = await _userController.RegisterUser(dto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<UserDTO>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedDTO = Assert.IsType<UserDTO>(okResult.Value);

        Assert.Equal(expectedUserDTO, returnedDTO);
    }

    #endregion using TheoryData V1

    #region using TheoryData V2

    //Use approach with TheoryData if:

    //The expected UserDTO is always a direct transformation of the input.
    //You want a more concise and easily maintainable test.

    public static TheoryData<UserRegistrationDTO> GetUserRegistrationDTOs()
    {
        return new TheoryData<UserRegistrationDTO>
        {
        new("Ketil", "Sveberg", "12345678", "Sveberg@gmail.com", "password"),
        new("Quyen", "Ho", "42534253", "Quyen99@gmail.com", "password2"),
        new("Nico", "Ho", "42534253", "Nico@gmail.com", "password3")
        };
    }


    [Theory]
    [MemberData(nameof(GetUserRegistrationDTOs))] // warning i tilfelle ikke er serializable: ikke primitive datatyper i DTO.

    public async Task RegisterUser_ShouldReturnOK_AndUserDTOV2(UserRegistrationDTO dto)
    {
        // arrange

        var dtNow = DateTime.UtcNow;

        var expectedUserDTO = new UserDTO(
            dto.FirstName,
            dto.LastName,
            dto.PhoneNumber,
            dto.Email,
            dtNow,
            dtNow);

        _userServiceMock.Setup(x => x.RegisterUserAsync(dto)).ReturnsAsync(expectedUserDTO);

        // Act

        var result = await _userController.RegisterUser(dto);

        // Assert

        var actionResult = Assert.IsType<ActionResult<UserDTO>>(result);
        var returnValue = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedDTO = Assert.IsType<UserDTO>(returnValue.Value);
        Assert.Equal(expectedUserDTO, returnedDTO);

    }
    #endregion using TheoryData V2

    #region using InlineData V3

    [Theory]
    [InlineData("Ketil", "Sveberg", "12345678", "Sveberg@gmail.com", "password")]
    [InlineData("Quyen", "Ho", "42534253", "Quyen99@gmail.com", "password2")]
    [InlineData("Nico", "Ho", "42534253", "Nico@gmail.com", "password3")]

    public async Task RegisterUser_ShouldReturnOK_AndUserDTOV3(string firstName, string lastName, string phoneNumber, string email, string password)
    {
        // Arrange

        var userRegistrationDTO = new UserRegistrationDTO(firstName, lastName, phoneNumber, email, password);

        _userServiceMock.Setup(x => x.RegisterUserAsync(userRegistrationDTO))
                    .ReturnsAsync(new UserDTO(
                        userRegistrationDTO.FirstName,
                        userRegistrationDTO.LastName,
                        userRegistrationDTO.PhoneNumber,
                        userRegistrationDTO.Email,
                        DateTime.UtcNow,
                        DateTime.UtcNow)
                    );

        // Act

        var res = await _userController.RegisterUser(userRegistrationDTO);

        // Assert

        var actionResult = Assert.IsType<ActionResult<UserDTO>>(res);
        var returnValue = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedDTO = Assert.IsType<UserDTO>(returnValue.Value);

        Assert.Equal(userRegistrationDTO.FirstName, returnedDTO.FirstName);
        Assert.Equal(userRegistrationDTO.LastName, returnedDTO.LastName);
        Assert.Equal(userRegistrationDTO.PhoneNumber, returnedDTO.PhoneNumber);
        Assert.Equal(userRegistrationDTO.Email, returnedDTO.Email);

        Assert.True(returnedDTO.DateCreated <= DateTime.UtcNow);
        Assert.True(returnedDTO.DateUpdated <= DateTime.UtcNow);
    }

    #endregion using InlineData V3

    #region using ClassData V4

    // ikke implementert. Er bare en annen måte å gjøre Theorydata på men med bruks av egen klasse (arv fra IEnumerable<object[]>) for testdata.

    #endregion using ClassData V4

    [Fact]
    public async Task RegisterUser_WhenUserRegistrationFails_ShouldReturnBadRequest400()
    {
        //Arrange
        UserRegistrationDTO dto = new("Nico", "Ho", "42534253", "Nico@gmail.com", "password3");

        _userServiceMock.Setup(x => x.RegisterUserAsync(dto)).ReturnsAsync((UserDTO?)null);

        //Act
        var res = await _userController.RegisterUser(dto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<UserDTO>>(res);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

        _userServiceMock.Verify(x => x.RegisterUserAsync(dto), Times.Once);
    }

    #endregion RegisterUserTests

    #region UserTests

    [Fact]

    public async Task GetUserById_WhenOk_ShouldReturnUser()
    {
        // Arrange

        Guid RandomGuidId = new Guid("345afc12-905c-40b2-b79b-6a98df2f9c72");

        UserDTO userDTO = new("Ketil", "Sveberg", "12345678", "Sveberg@.gmail.com", new DateTime(2024, 10, 17, 02, 50, 00), new DateTime(2024, 10, 17, 02, 52, 30));

        _userServiceMock.Setup(x => x.GetUserByIdAsync(RandomGuidId)).ReturnsAsync(userDTO);
        _mockUserClaim.Setup(u => u.FindFirst(JwtRegisteredClaimNames.Sub)).Returns(new Claim(JwtRegisteredClaimNames.Sub, "345afc12-905c-40b2-b79b-6a98df2f9c72"));
        // Act

        var res = await _userController.GetUserById(RandomGuidId);

        // Assert

        var actionResult = Assert.IsType<ActionResult<UserDTO>>(res);  // det vi får tilbake (ActionResult<DTO>)
        var returnValue = Assert.IsType<OkObjectResult>(actionResult.Result);// sjekker om OKresult  er ok
        var dto = Assert.IsType<UserDTO>(returnValue.Value);

        Assert.Equal(dto.FirstName, userDTO.FirstName);
        Assert.Equal(dto.LastName, userDTO.LastName);
        Assert.Equal(dto.PhoneNumber, userDTO.PhoneNumber);
        Assert.Equal(dto.Email, userDTO.Email);
        Assert.Equal(dto.DateCreated, userDTO.DateCreated);
        Assert.Equal(dto.DateUpdated, userDTO.DateUpdated);

        _userServiceMock.Verify(x => x.GetUserByIdAsync(RandomGuidId), Times.Once);
    }

    [Fact]
    public async Task GetUserById_WhenUserNotExist_ShouldReturnNotFound()
    {
        // Arrange

        Guid RandomGuidId = new Guid("345afc12-905c-40b2-b79b-6a98df2f9c72");

        _userServiceMock.Setup(x => x.GetUserByIdAsync(RandomGuidId)).ReturnsAsync((UserDTO?)null);
        _mockUserClaim.Setup(u => u.FindFirst(JwtRegisteredClaimNames.Sub)).Returns(new Claim(JwtRegisteredClaimNames.Sub, "345afc12-905c-40b2-b79b-6a98df2f9c72"));
        // Act

        var res = await _userController.GetUserById(RandomGuidId);

        // Assert

        var actionResult = Assert.IsType<ActionResult<UserDTO>>(res);
        var returnValue = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal($"No user with Id {RandomGuidId} was found", returnValue.Value);

        _userServiceMock.Verify(x => x.GetUserByIdAsync(RandomGuidId), Times.Once);
    }

    [Fact]
    public async Task GetUserById_WhenUserNotAuthorized_ShouldReturnUnauthorized()
    {
        // Arrange

        Guid RandomGuidId = new Guid("345afc12-905c-40b2-b79b-6a98df2f9c72");

        _userServiceMock.Setup(x => x.GetUserByIdAsync(RandomGuidId)).ReturnsAsync((UserDTO?)null);
        _mockUserClaim.Setup(u => u.FindFirst(JwtRegisteredClaimNames.Sub)).Returns(new Claim(JwtRegisteredClaimNames.Sub, "345afc12-905c-40b2-b79b-6a98df2f9c73"));
        // Act

        var res = await _userController.GetUserById(RandomGuidId);

        // Assert

        var actionResult = Assert.IsType<ActionResult<UserDTO>>(res);
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
        Assert.Equal($"Not authorized to get this user", unauthorizedResult.Value);

        _userServiceMock.Verify(x => x.GetUserByIdAsync(RandomGuidId), Times.Never);

    }

    [Fact]
    public async Task GetUserById_WhenRetrievingUser_WhenIsAdmin_ShouldReturnUserDTO()
    {
        // Arrange
        Guid id = new("894efa5f-d594-4064-9200-fad11766bd83");
        UserDTO userDTO = new(
        "Ketil",
        "Sveberg",
        "12345678",
        "Sveberg@gmail.com",
        new DateTime(2024, 10, 17, 02, 50, 00),
        new DateTime(2024, 10, 17, 02, 52, 30));

        _userServiceMock.Setup(x => x.GetUserByIdAsync(id)).ReturnsAsync(userDTO);

        var claims = new List<Claim>
        {
        new Claim(JwtRegisteredClaimNames.Sub, "ae045d60-8b4e-4b5e-b229-6a8c8a97bcfd"),
        new Claim(ClaimTypes.Role, "Admin")
        };

        _userController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims))
            }
        };

        // Act
        var res = await _userController.GetUserById(id);

        // Assert
        var actionResult = Assert.IsType<ActionResult<UserDTO>>(res);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var dto = Assert.IsType<UserDTO>(okResult.Value);

        Assert.Equal(dto.FirstName, userDTO.FirstName);
        Assert.Equal(dto.LastName, userDTO.LastName);
        Assert.Equal(dto.PhoneNumber, userDTO.PhoneNumber);
        Assert.Equal(dto.Email, userDTO.Email);
        Assert.Equal(dto.DateCreated, userDTO.DateCreated);
        Assert.Equal(dto.DateUpdated, userDTO.DateUpdated);

        _userServiceMock.Verify(x => x.GetUserByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_WhenUpdatingUserWithValidUser_ShouldReturnUpdateUserDTO()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        UserUpdateDTO userUpdateDTO = new
        (
            "Ketil",
            "Sveberg",
            "12345678",
            "Sveberg@gmail.com"
        );

        UserDTO userDTO = new
        (
            "Ketil",
            "Sveberg",
            "12345678",
            "Sveberg@gmail.com",
            DateTime.Now,
            DateTime.Now
        );

        _userServiceMock.Setup(x => x.UpdateUserAsync(id, userUpdateDTO)).ReturnsAsync(userDTO);
        _mockUserClaim.Setup(x => x.FindFirst(JwtRegisteredClaimNames.Sub)).Returns(new Claim(JwtRegisteredClaimNames.Sub, id.ToString()));

        // Act 

        var res = await _userController.UpdateUserById(id, userUpdateDTO);

        // Assert

        var actionResult = Assert.IsType<ActionResult<UserDTO>>(res);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var dto = Assert.IsType<UserDTO>(okResult.Value);

        Assert.Equal(userUpdateDTO.FirstName, userDTO.FirstName);
        Assert.Equal(userUpdateDTO.LastName, userDTO.LastName); 
        Assert.Equal(userUpdateDTO.PhoneNumber, userDTO.PhoneNumber);
        Assert.Equal(userUpdateDTO.Email, userDTO.Email);

        _userServiceMock.Verify(x => x.UpdateUserAsync(id, userUpdateDTO), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_WhenUpdatingUserAsAdmin_ShouldReturnUpdatedUserDTO()
    {
        // Arrange
        Guid idToUpdate = new("ae045d60-8b4e-4b5e-b229-6a8c8a97bcfd");

        UserUpdateDTO userUpdateDTO = new
        (
            "Ketil",
            "Sveberg",
            "12345678",
            "Sveberg@gmail.com"
        );

        UserDTO userDTO = new
        (
            "Ketil",
            "Sveberg",
            "12345678",
            "Sveberg@gmail.com",
            DateTime.Now,
            DateTime.Now
        );

        _userServiceMock.Setup(x => x.UpdateUserAsync(idToUpdate, userUpdateDTO)).ReturnsAsync(userDTO);

        var claims = new List<Claim>
        {
        new Claim(JwtRegisteredClaimNames.Sub, "ae045d60-8b4e-4b5e-b229-6a8c8a97bcf1"),
        new Claim(ClaimTypes.Role, "Admin")
        };

        _userController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims))
            }
        };

        // Act 

        var res = await _userController.UpdateUserById(idToUpdate, userUpdateDTO);

        // Assert

        var actionResult = Assert.IsType<ActionResult<UserDTO>>(res);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var dto = Assert.IsType<UserDTO>(okResult.Value);

        Assert.NotNull(res);
        Assert.Equal(userUpdateDTO.FirstName, userDTO.FirstName);
        Assert.Equal(userUpdateDTO.LastName, userDTO.LastName);
        Assert.Equal(userUpdateDTO.PhoneNumber, userDTO.PhoneNumber);
        Assert.Equal(userUpdateDTO.Email, userDTO.Email);

        _userServiceMock.Verify(x => x.UpdateUserAsync(idToUpdate, userUpdateDTO), Times.Once);
    }


    [Fact]
    public async Task UpdateUser_WhenUpdatingUserWithUnauthorizedUser_ShouldReturnUnauthorized()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        UserUpdateDTO userUpdateDTO = new
        (
            "Ketil",
            "Sveberg",
            "12345678",
            "Sveberg@gmail.com"
        );

        //_userServiceMock.Setup(x => x.UpdateUserAsync(id, userUpdateDTO)).ReturnsAsync(userDTO);
        _mockUserClaim.Setup(x => x.FindFirst(JwtRegisteredClaimNames.Sub)).Returns(new Claim(JwtRegisteredClaimNames.Sub, "ae045d60-8b4e-4b5e-b229-6a8c8a97bcfd"));

        // Act 

        var res = await _userController.UpdateUserById(id, userUpdateDTO);

        // Assert

        var actionResult = Assert.IsType<ActionResult<UserDTO>>(res);
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);

        Assert.Equal("Not authorized to update this user", unauthorizedResult.Value);


        _userServiceMock.Verify(x => x.UpdateUserAsync(id, userUpdateDTO), Times.Never);
    }

    [Fact]  

    public async Task UpdateUser_WhenAuthenticatedUserIsNotFound_ShouldReturnNotFound()
    {
        // Arrange

        Guid userId = Guid.NewGuid();

        UserUpdateDTO userUpdateDTO = new
        (
            "Ketil",
            "Sveberg",
            "12345678",
            "Sveberg@gmail.com"
        );

        _userServiceMock.Setup(x => x.UpdateUserAsync(userId, userUpdateDTO)).ReturnsAsync((UserDTO?)null);
        _mockUserClaim.Setup(x => x.FindFirst(JwtRegisteredClaimNames.Sub)).Returns(new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()));

        // Act

        var res = await _userController.UpdateUserById(userId, userUpdateDTO);


        // Assert

        var actionResult = Assert.IsType<ActionResult<UserDTO>>(res);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal($"Could not Update User with Id {userId}", notFoundResult.Value);

        _userServiceMock.Verify(x => x.UpdateUserAsync(userId, userUpdateDTO), Times.Once());  

    }
    #endregion

}
