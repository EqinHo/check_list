using Checklist_API.Features.Users.Controller;
using Checklist_API.Features.Users.DTOs;
using Checklist_API.Features.Users.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Checklist.UnitTests.ControllersTests;
public class UserControllerTests
{
    private readonly UserController _userController;
    private readonly Mock<IUserService> _userServiceMock = new Mock<IUserService>();
    private readonly Mock<ILogger<UserController>> _loggerMock = new Mock<ILogger<UserController>>();

    public UserControllerTests()
    {
        _userController = new UserController(_userServiceMock.Object, _loggerMock.Object);
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

        _userServiceMock.Setup(x => x.GetAllAsync(page, pageSize)).ReturnsAsync(dtos);

        // Act

        var res = await _userController.GetAll(page, pageSize);

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

        _userServiceMock.Setup(x => x.GetAllAsync(page, pageSize)).ReturnsAsync(() => null!);

        // Act

        var res = await _userController.GetAll(page, pageSize);

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

    public async Task RegisterUserAsync_WhenOK_ShouldReturnUserDTO(UserRegistrationDTO dto, UserDTO expectedUserDTO)
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
 
    public async Task RegisterUserAsync_ShouldReturnOK_AndUserDTOV2(UserRegistrationDTO dto)
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

    public async Task RegisterUserAsync_ShouldReturnOK_AndUserDTOV3(string firstName, string lastName, string phoneNumber, string email, string password)
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

    #endregion RegisterUserTests

    [Fact]
    public async Task RegisterUserAsync_WhenUserRegistrationFails_ShouldReturnBadRequest400()
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
}
