using Checklist_API.Features.Common.Interfaces;
using Checklist_API.Features.Users.DTOs;
using Checklist_API.Features.Users.Entity;

namespace Checklist_API.Features.Users.Mappers;

public class UserUpdateMapper : IMapper<User, UserUpdateDTO>
{
    public UserUpdateDTO MapToDTO(User entity)
    {
        throw new NotImplementedException();
    }

    public User MapToEntity(UserUpdateDTO dto)
    {
        return new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email
        };
          
    }
}
