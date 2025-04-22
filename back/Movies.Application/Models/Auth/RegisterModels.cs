namespace Movies.Application.Models.Auth;

public class RegisterDto
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
}

public enum RegisterResponseStatus
{
    Success, UserExists, BadPassword, BadUsername
}