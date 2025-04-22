using Microsoft.EntityFrameworkCore;
using Moq;
using Movies.Application.Helpers;
using Movies.Application.Models.Auth;
using Movies.Application.Services.Implementations;
using Movies.Domain.Entities;
using Movies.Domain.Enums;
using Movies.Domain.Repositories;
using Xunit;

namespace Movies.Application.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ITokenGenerator> _mockTokenGenerator;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockTokenGenerator = new Mock<ITokenGenerator>();
        
        _authService = new AuthService(_mockUserRepository.Object, _mockTokenGenerator.Object);
    }

    #region Register Test
    
    [Fact]
    public async Task RegisterUser_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "testuser",
            Password = "password123",
            FullName = "Test User"
        };

        var users = new List<User>().AsQueryable();
        var asyncUsers = new TestAsyncEnumerable<User>(users);
        _mockUserRepository.Setup(x => x.GetAllUsers())
            .Returns(asyncUsers);

        _mockUserRepository.Setup(x => x.AddUser(It.IsAny<User>()))
            .ReturnsAsync(It.IsAny<User>());
        // It.IsAny<User>() means it will accept any User object being passed.

        // Act
        var result = await _authService.RegisterUser(registerDto);

        // Assert
        Assert.Equal(RegisterResponseStatus.Success, result);
        _mockUserRepository.Verify(x => x.AddUser(It.Is<User>(u => 
            u.Username == registerDto.Username.ToLower() &&
            u.FullName == registerDto.FullName &&
            u.Role == UserRoles.User &&
            !u.IsDelete)), Times.Once);
    }

    [Fact]
    public async Task RegisterUser_WithExistingUsername_ReturnsUserExists()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "existinguser",
            Password = "password123",
            FullName = "Test User"
        };

        var existingUser = new User
        {
            Username = registerDto.Username,
            Password = "hashedpassword",
            FullName = "Existing User",
            Role = UserRoles.User
        };
        var users = new List<User> { existingUser }.AsQueryable();
            var asyncUsers = new TestAsyncEnumerable<User>(users);
        _mockUserRepository.Setup(x => x.GetAllUsers())
            .Returns(asyncUsers);

        // Act
        var result = await _authService.RegisterUser(registerDto);

        // Assert
        Assert.Equal(RegisterResponseStatus.UserExists, result);
        _mockUserRepository.Verify(x => x.AddUser(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterUser_WithEmptyUsername_ReturnsBadUsername()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "",
            Password = "password123",
            FullName = "Test User"
        };

        // Act
        var result = await _authService.RegisterUser(registerDto);

        // Assert
        Assert.Equal(RegisterResponseStatus.BadUsername, result);
        _mockUserRepository.Verify(x => x.AddUser(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterUser_WithWhitespaceUsername_ReturnsBadUsername()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "   ",
            Password = "password123",
            FullName = "Test User"
        };

        // Act
        var result = await _authService.RegisterUser(registerDto);

        // Assert
        Assert.Equal(RegisterResponseStatus.BadUsername, result);
        _mockUserRepository.Verify(x => x.AddUser(It.IsAny<User>()), Times.Never);
    }

    #endregion
    
    #region Login Test
    
    [Fact]
        public async Task LoginUser_UserNotFound_ReturnsUserNotFoundStatus()
        {
            // Arrange: create a login DTO for a user that does not exist.
            var loginDto = new LoginDto 
            { 
                Username = "nonexistentuser", 
                Password = "anyPassword" 
            };

            // Return an empty async queryable.
            var users = new List<User>().AsQueryable();
            var asyncUsers = new TestAsyncEnumerable<User>(users);
            _mockUserRepository
                .Setup(x => x.GetAllUsers())
                .Returns(asyncUsers);

            // Act
            var result = await _authService.LoginUser(loginDto);

            // Assert: We expect the response status to be UserNotFound.
            Assert.Equal(LoginResponseStatus.UserNotFound, result.Status);
        }

        [Fact]
        public async Task LoginUser_InvalidPassword_ReturnsInvalidCredentialsStatus()
        {
            // Arrange: set up the login dto and create a user with a known password.
            var loginDto = new LoginDto 
            { 
                Username = "testuser", 
                Password = "wrongPassword" 
            };

            // Suppose the user's actual password (when hashed) is from "correctPassword".
            var correctPassword = "correctPassword";
            var hashedCorrectPassword = Hasher.HashPassword(correctPassword);

            var user = new User 
            { 
                Username = "testuser", 
                Password = hashedCorrectPassword 
            };

            // Wrap the user in an async queryable.
            var users = new List<User> { user }.AsQueryable();
            var asyncUsers = new TestAsyncEnumerable<User>(users);
            _mockUserRepository
                .Setup(x => x.GetAllUsers())
                .Returns(asyncUsers);

            // Act
            var result = await _authService.LoginUser(loginDto);

            // Assert: Since the password doesn't match, status should be InvalidCredentials.
            Assert.Equal(LoginResponseStatus.InvalidCredentials, result.Status);
        }

        [Fact]
        public async Task LoginUser_ValidCredentials_ReturnsSuccessAndToken()
        {
            // Arrange: create a login dto with valid credentials.
            var loginDto = new LoginDto 
            { 
                Username = "testuser", 
                Password = "password123" 
            };

            var hashedPassword = Hasher.HashPassword("password123");
            var user = new User 
            { 
                Username = loginDto.Username, 
                Password = hashedPassword 
            };

            // Wrap the user in an async queryable.
            var users = new List<User> { user }.AsQueryable();
            var asyncUsers = new TestAsyncEnumerable<User>(users);
            _mockUserRepository
                .Setup(x => x.GetAllUsers())
                .Returns(asyncUsers);
            _mockTokenGenerator.Setup(t => t.GenerateJwtToken(It.IsAny<User>()))
                .Returns("dummy-token");

            // Act
            var result = await _authService.LoginUser(loginDto);

            // Assert: We expect a success status and a non-empty token.
            Assert.Equal(LoginResponseStatus.Success, result.Status);
            Assert.False(string.IsNullOrEmpty(result.Token));
        }
    
    #endregion
} 