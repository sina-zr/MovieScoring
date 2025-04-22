using Movies.Application.Models.Auth;

namespace Movies.Application.Services.Interfaces;

public interface IAuthService
{
    Task<RegisterResponseStatus> RegisterUser(RegisterDto registerDto);
    Task<LoginResponse> LoginUser(LoginDto loginDto);
}