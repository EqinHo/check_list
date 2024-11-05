using Checklist_API.Features.Common.Interfaces;
using Checklist_API.Features.Users.DTOs;
using Checklist_API.Features.Users.Entity;
using Checklist_API.Features.Users.Mappers;

namespace Checklist.UnitTests.MappersTests;
public class UserRegistrationMapperTests
{
    private readonly IMapper<User, UserRegistrationDTO> _userRegistrationMapper = new UserRegistrationMapper();

    [Fact]
    public void MapToEntity_WhenMappingRegistrationDTOToUser_ShouldReturnUser()
    {
        // Arrange

        UserRegistrationDTO userRegistrationDTO = new(
            "Ketil",
            "Sveberg",
            "11112222",
            "ketilsveberg@gmail.com",
            "123456"          
            );

        // Act

        var user = _userRegistrationMapper.MapToEntity( userRegistrationDTO );

        // Assert

        Assert.NotNull( user );
        Assert.Equal(user.FirstName, userRegistrationDTO.FirstName);
        Assert.Equal(user.LastName, userRegistrationDTO.LastName);
        Assert.Equal(user.PhoneNumber, userRegistrationDTO.PhoneNumber);
        Assert.Equal(user.Email, userRegistrationDTO.Email);

        Assert.True(user.DateCreated > DateTime.Now.AddSeconds(-1) && user.DateCreated <= DateTime.Now, "DateCreated is not set correctly");
        Assert.True(user.DateUpdated > DateTime.Now.AddSeconds(-1) && user.DateUpdated <= DateTime.Now, "DateUpdated is not set correctly");
    }
}
