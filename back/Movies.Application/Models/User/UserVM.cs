using Movies.Domain.Enums;

namespace Movies.Application.Models.User;

public class AddUserDto
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string? FullName { get; set; }
    public UserRoles? Role { get; set; }
}

public enum AddUserResponse
{
    Success, EmptyUsernameOrPassword, UsernameExists, UnKnownError
}