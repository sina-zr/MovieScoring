using Moq;
using Movies.Application.Models.User;
using Movies.Application.Services.Implementations;
using Movies.Application.Services.Interfaces;
using Movies.Domain.Entities;
using Movies.Domain.Repositories;

namespace Movies.Application.Tests.Services;

public class UserManagementServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;

    private readonly IUserManagementService _sut;
    
    public UserManagementServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        
        _sut = new UserManagementService(_mockUserRepository.Object);
    }

    #region AddUser

    /// <summary>
    /// test when username already exists in database
    /// </summary>
    [Fact]
    public async Task AddUser_UsernameExists_ShouldReturnExistsStatus()
    {
        // Arrange
        var dto = new AddUserDto()
        {
            Username = "test",
            Password = "pass",
            FullName = "John Doe"
        };
        var users = new List<User>()
        {
            new User() { Id = 1, Username = "test", Password = "1234" }
        };
        _mockUserRepository.Setup(x => x.GetAllUsers())
            .Returns(new TestAsyncEnumerable<User>(users));
        User? addedUser = null;
        _mockUserRepository.Setup(x=> x.AddUser(It.IsAny<User>()))
            .Returns((User u) =>
            {
                addedUser = u;
                return Task.FromResult(u);
            });

        // Act
        var response = await _sut.AddUser(dto);

        // Assert
        Assert.Equal(AddUserResponse.UsernameExists, response);
        _mockUserRepository.Verify(x => x.GetAllUsers(), Times.Once);
        _mockUserRepository.Verify(x => x.AddUser(It.IsAny<User>()), Times.Never);
        Assert.Null(addedUser);
    }
    
    /// <summary>
    /// username empty, pass ok
    /// username ok, pass empty
    /// both empty
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    [Theory]
    [InlineData("", "pass")]
    [InlineData(null, "pass")]
    [InlineData("test", "")]
    [InlineData("test", null)]
    [InlineData("", null)]
    public async Task AddUser_InvalidUsernameOrPass_ShouldReturnEmptyStatus(string username, string password)
    {
        // Arrange
        var dto = new AddUserDto()
        {
            Username = username,
            Password = password,
            FullName = "John Doe"
        };
        _mockUserRepository.Setup(x=> x.AddUser(It.IsAny<User>()))
            .Returns((User u) => Task.FromResult(u));

        // Act
        var response = await _sut.AddUser(dto);

        // Assert
        Assert.Equal(AddUserResponse.EmptyUsernameOrPassword, response);
        _mockUserRepository.Verify(x => x.AddUser(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task AddUser_ValidUser_ShouldReturnSuccess()
    {
        // Arrange
        var dto = new AddUserDto()
        {
            Username = "test",
            Password = "pass",
            FullName = "John Doe"
        };
        var users = new List<User>()
        {
            new User() { Id = 1, Username = "test2", Password = "someHash" }
        };
        _mockUserRepository.Setup(x => x.GetAllUsers())
            .Returns(new TestAsyncEnumerable<User>(users));
        User? addedUser = null;
        _mockUserRepository.Setup(x=> x.AddUser(It.IsAny<User>()))
            .Returns((User u) =>
            {
                addedUser = u;
                return Task.FromResult(u);
            });

        // Act
        var response = await _sut.AddUser(dto);

        // Assert
        Assert.Equal(AddUserResponse.Success, response);
        _mockUserRepository.Verify(x => x.GetAllUsers(), Times.Once);
        _mockUserRepository.Verify(x => x.AddUser(It.IsAny<User>()), Times.Once);
        Assert.NotNull(addedUser);
        Assert.Equal(dto.Username.Trim().ToLower(), addedUser!.Username);
    }

    #endregion
    
    
    
}