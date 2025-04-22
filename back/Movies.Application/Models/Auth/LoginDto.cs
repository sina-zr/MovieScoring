namespace Movies.Application.Models.Auth;

public class LoginDto
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class LoginResponse
{
    public string? Token { get; set; } = string.Empty;
    public LoginResponseStatus Status { get; set; }
}
 
public enum LoginResponseStatus
{
    Success,
    InvalidCredentials,
    UserNotFound
}
 
