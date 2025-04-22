using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Movies.Application.Models.Auth;
using Movies.Application.Services.Interfaces;

namespace Movies.Api.Controllers;

[ApiController]
[Route("/api")]
[ApiExplorerSettings(IgnoreApi = false)]
[EnableRateLimiting("FixedWindow")]
public class AccountController : ControllerBase
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Endpoint for user register
    /// </summary>
    /// <param name="registerDto"></param>
    /// <response code="400">User Already Exists</response>
    /// <response code="400">Bad Username</response>
    /// <response code="400">Bad Password</response>
    /// <response code="500">Server Error</response>
    /// <returns>Http Status Codes</returns>
    [HttpPost("[action]")]
    [EnableRateLimiting("FixedWindow")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var response = await _authService.RegisterUser(new RegisterDto()
        {
            Username = registerDto.Username,
            Password = registerDto.Password,
            FullName = registerDto.FullName
        });

        switch (response)
        {
            case RegisterResponseStatus.Success:
                return Ok();
            case RegisterResponseStatus.UserExists:
                return BadRequest("User already exists");
            case RegisterResponseStatus.BadPassword:
                return BadRequest("Bad Password");
            case RegisterResponseStatus.BadUsername:
                return BadRequest("Bad Username");
            default:
                return StatusCode(500);
        }
    }
    
    /// <summary>
    /// Endpoint for user login
    /// </summary>
    /// <param name="loginDto"></param>
    /// <response code="۲00">token</response>
    /// <response code="400">Invalid credentials</response>
    /// <response code="500">Server Error</response>
    /// <returns>HttpStatus with message inside. token if ok</returns>
    [HttpPost("[action]")]
    [EnableRateLimiting("FixedWindow")]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var response = await _authService.LoginUser(new LoginDto()
        {
            Username = loginDto.Username,
            Password = loginDto.Password
        });

        switch (response.Status)
        {
            case LoginResponseStatus.Success:
                return Ok(response.Token);
            case LoginResponseStatus.InvalidCredentials:
                return BadRequest("Invalid credentials");
            default:
                return StatusCode(500);
        }
    }
}