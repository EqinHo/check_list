using Checklist_API.Extensions;
using Checklist_API.Features.Common.Interfaces;
using Checklist_API.Features.Users.DTOs;
using Checklist_API.Features.Users.Entity;
using Checklist_API.Features.Users.Mappers;
using Checklist_API.Features.Users.Repository.Interfaces;
using Checklist_API.Features.Users.Service.Interfaces;
using static Checklist_API.Extensions.CustomExceptions;

namespace Checklist_API.Features.Users.Service;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;
    private readonly IMapper<User, UserDTO> _userMapper;
    private readonly IMapper<User, UserRegistrationDTO> _userRegistrationMapper;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger, IMapper<User, UserDTO> userMapper,
                        IMapper<User, UserRegistrationDTO> userRegistrationMapper)
    {
        _userRepository = userRepository;
        _logger = logger;
        _userMapper = userMapper;
        _userRegistrationMapper = userRegistrationMapper;
    }

    public async Task<IEnumerable<UserDTO>> GetAllAsync(int page, int pageSize)
    {
        _logger.LogInformation("getting all users");

        var res = await _userRepository.GetAllAsync(page, pageSize);

        var dtos = res.Select(user => _userMapper.MapToDTO(user)).ToList();
        return dtos;
    }

    public Task<UserDTO?> GetByIdAsync(Guid UserId)
    {
        throw new NotImplementedException();
    }

    public Task<UserDTO?> UpdateAsync(Guid id, UserDTO dto)
    {
        throw new NotImplementedException();
    }

    public Task<UserDTO?> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<UserDTO?> RegisterUserAsync(UserRegistrationDTO dto)
    {
        _logger.LogDebug("Registering new user: {email}", dto.Email);

        var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
        if (existingUser != null)
        {
            _logger.LogDebug("User already exist: {Email}", dto.Email);

            throw new UserAlreadyExistsException();
        }       

        var user = _userRegistrationMapper.MapToEntity(dto);

        user.Id = UserId.NewId;
        user.Salt = BCrypt.Net.BCrypt.GenerateSalt();
        user.HashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var res = await _userRepository.RegisterAsync(user);

        return res != null ? _userMapper.MapToDTO(res) : null;
    }
}
