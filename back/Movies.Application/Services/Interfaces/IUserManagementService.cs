using Movies.Application.Models.User;
using Movies.Domain.Entities;

namespace Movies.Application.Services.Interfaces;

public interface IUserManagementService
{
    // AddUser
    Task<AddUserResponse> AddUser(AddUserDto addUserDto);

    // DeleteUSer
    // EditUser
}