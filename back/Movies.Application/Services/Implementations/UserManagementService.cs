using Microsoft.EntityFrameworkCore;
using Movies.Application.Helpers;
using Movies.Application.Models.User;
using Movies.Application.Services.Interfaces;
using Movies.Domain.Entities;
using Movies.Domain.Enums;
using Movies.Domain.Repositories;

namespace Movies.Application.Services.Implementations;

public class UserManagementService : IUserManagementService
{
    private readonly IUserRepository _userRepository;

    public UserManagementService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<AddUserResponse> AddUser(AddUserDto addUserDto)
    {
        if (string.IsNullOrEmpty(addUserDto.Username) || string.IsNullOrEmpty(addUserDto.Password))
        {
            return AddUserResponse.EmptyUsernameOrPassword;
        }
        
        addUserDto.Username = addUserDto.Username.ToLower();
        var user = await _userRepository.GetAllUsers()
            .FirstOrDefaultAsync(u => u.Username == addUserDto.Username);
        if (user != null)
        {
            return AddUserResponse.UsernameExists;
        }
        
        var hashedPassword = Hasher.HashPassword(addUserDto.Password);

        try
        {
            await _userRepository.AddUser(new User()
            {
                Username = addUserDto.Username,
                Password = hashedPassword,
                FullName = addUserDto.FullName ?? "",
                Role = addUserDto.Role ?? UserRoles.User,
                IsDelete = false,
                CreateDate = DateTime.UtcNow
            });

            return AddUserResponse.Success;
        }
        catch (Exception e)
        {
            // log
            return AddUserResponse.UnKnownError;
        }
    }
}