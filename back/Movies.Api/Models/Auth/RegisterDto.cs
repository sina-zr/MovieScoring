namespace Movies.Api.Models.Auth;

public class RegisterDto
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
}