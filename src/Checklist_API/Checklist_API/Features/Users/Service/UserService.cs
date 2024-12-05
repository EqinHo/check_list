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
    private readonly IMapper<User, UserUpdateDTO> _userUpdateMapper;
    private readonly IMapper<User, UserRegistrationDTO> _userRegistrationMapper;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger, IMapper<User, UserDTO> userMapper, IMapper<User, UserUpdateDTO> userUpdateMapper,
                        IMapper<User, UserRegistrationDTO> userRegistrationMapper)
    {
        _userRepository = userRepository;
        _logger = logger;
        _userMapper = userMapper;
        _userUpdateMapper = userUpdateMapper;
        _userRegistrationMapper = userRegistrationMapper;
    }

    public async Task<IEnumerable<UserDTO>> GetAllUsersAsync(int page, int pageSize)
    {
        _logger.LogInformation("Retrieving all users");

        var res = await _userRepository.GetAllUsersAsync(page, pageSize);

        var dtos = res.Select(user => _userMapper.MapToDTO(user)).ToList();
        return dtos;
    }

    public async Task<UserDTO?> GetUserByIdAsync(Guid userId)
    {
        _logger.LogInformation("Retrieving user by ID : {id}", userId);

        var user = await _userRepository.GetUserByIdAsync(new UserId(userId));
        if (user == null)
        {
            _logger.LogWarning($"User with ID: {userId} not found.");
            return null;
        }

        // Bruk en mapper for transformasjon
        var userDto = _userMapper.MapToDTO(user);
        return userDto;
    }

    public async Task<UserDTO?> UpdateUserAsync(Guid id, UserUpdateDTO dto)
    {
        _logger.LogInformation("Update user With Id : {id}", id);

        var user = _userUpdateMapper.MapToEntity(dto);
        var res = await _userRepository.UpdateUserAsync(new UserId(id), user);

        return res != null ? _userMapper.MapToDTO(res) : null;
    }

    public async Task<UserDTO?> DeleteUserAsync(Guid id)
    {
        _logger.LogInformation("Delete user with Id : {id}", id);

        var user = await _userRepository.DeleteUserAsync(new UserId(id));
        return user != null ? _userMapper.MapToDTO(user) : null;
    }

    public async Task<UserDTO?> RegisterUserAsync(UserRegistrationDTO dto)
    {
        _logger.LogDebug("Registering new user: {email}", dto.Email);

        var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);
        if (existingUser != null)
        {
            _logger.LogDebug("User already exist: {Email}", dto.Email);

            throw new UserAlreadyExistsException();
        }       

        var user = _userRegistrationMapper.MapToEntity(dto);

        user.Id = UserId.NewId;
        user.Salt = BCrypt.Net.BCrypt.GenerateSalt();
        user.HashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var res = await _userRepository.RegisterUserAsync(user);

        return res != null ? _userMapper.MapToDTO(res) : null;
    }

}
