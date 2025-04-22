using Microsoft.EntityFrameworkCore;
using Movies.Application.Helpers;
using Movies.Application.Models.Auth;
using Movies.Application.Services.Interfaces;
using Movies.Domain.Entities;
using Movies.Domain.Enums;
using Movies.Domain.Repositories;

namespace Movies.Application.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenGenerator _tokenGenerator;

    public AuthService(IUserRepository userRepository, ITokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
    }
    
    public async Task<RegisterResponseStatus> RegisterUser(RegisterDto registerDto)
    {
        registerDto.Username = registerDto.Username.Trim().ToLower();

        if (string.IsNullOrEmpty(registerDto.Username))
        {
            return RegisterResponseStatus.BadUsername;
        }
        
        var existingUser = await _userRepository.GetAllUsers()
            .FirstOrDefaultAsync(u => u.Username == registerDto.Username);
        if (existingUser != null)
        {
            return RegisterResponseStatus.UserExists;
        }
        
        var passwordHashed = Hasher.HashPassword(registerDto.Password);
        await _userRepository.AddUser(new User()
        {
            Username = registerDto.Username,
            Password = passwordHashed,
            FullName = registerDto.FullName,
            Role = UserRoles.User,
            CreateDate = DateTime.UtcNow,
            IsDelete = false
        });

        return RegisterResponseStatus.Success;
    }

    public async Task<LoginResponse> LoginUser(LoginDto loginDto)
    {
        var user = await _userRepository.GetAllUsers()
            .FirstOrDefaultAsync(u => u.Username == loginDto.Username);
        if (user == null)
        {
            return new LoginResponse()
            {
                Status = LoginResponseStatus.UserNotFound
            };
        }
        
        var passwordHashed = Hasher.HashPassword(loginDto.Password);
        if (user.Password != passwordHashed)
        {
            return new LoginResponse()
            {
                Status = LoginResponseStatus.InvalidCredentials
            };
        }

        var token = _tokenGenerator.GenerateJwtToken(user);
        return new LoginResponse()
        {
            Token = token,
            Status = LoginResponseStatus.Success
        };
    }
}