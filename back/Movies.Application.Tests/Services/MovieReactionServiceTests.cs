using Moq;
using Movies.Application.Models.Movie;
using Movies.Application.Services.Implementations;
using Movies.Application.Services.Interfaces;
using Movies.Domain.Entities;
using Movies.Domain.Repositories;

namespace Movies.Application.Tests.Services;

public class MovieReactionServiceTests
{
    private readonly Mock<IUserRateRepository> _mockUserRateRepository;
    private readonly Mock<ICommentRepository> _mockCommentRepository;
    
    private readonly IMovieReactionService _sut;
    
    public MovieReactionServiceTests()
    {
        _mockUserRateRepository = new Mock<IUserRateRepository>();
        _mockCommentRepository = new Mock<ICommentRepository>();
        
        _sut = new MovieReactionService(_mockUserRateRepository.Object, _mockCommentRepository.Object);
    }

    #region AddScroe

    /// <summary>
    /// Test all bad score input conditions
    /// </summary>
    /// <param name="score"></param>
    [Theory]
    [InlineData(100)]
    [InlineData(634)]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task AddScore_InvalidScore_ShouldReturnInvalidScore(int score)
    {
        // Arrange
        ulong movieId = 1;
        ulong userId = 1;

        _mockUserRateRepository.Setup(x => x.GetAllUserRates())
            .Returns(new TestAsyncEnumerable<UserRate>(new List<UserRate>()));
        _mockUserRateRepository.Setup(x => x.AddUserRate(It.IsAny<UserRate>()))
            .Returns((UserRate ur) => Task.FromResult(ur));

        // Act
        var status = await _sut.AddMovieScore(movieId, userId, score);

        // Arrange
        Assert.Equal(ScoreMovieResponse.InvalidScore, status);
        _mockUserRateRepository.Verify(x => x.GetAllUserRates(), Times.Never);
        _mockUserRateRepository.Verify(x => x.AddUserRate(It.IsAny<UserRate>()), Times.Never);
    }

    /// <summary>
    /// Condition where user already has scored this movie
    /// </summary>
    [Fact]
    public async Task AddScore_ExistingMovieScore_ShouldReturnAlreadyScored()
    {
        // Arrange
        ulong movieId = 1;
        ulong userId = 1;
        var score = 5;

        var userRates = new List<UserRate>()
        {
            new UserRate() { MovieId = movieId, UserId = userId, Score = 3 }
        };
        _mockUserRateRepository.Setup(x => x.GetAllUserRates())
            .Returns(new TestAsyncEnumerable<UserRate>(userRates));
        _mockUserRateRepository.Setup(x => x.AddUserRate(It.IsAny<UserRate>()))
            .Returns((UserRate ur) => Task.FromResult(ur));

        // Act
        var status = await _sut.AddMovieScore(movieId, userId, score);

        // Arrange
        Assert.Equal(ScoreMovieResponse.AlreadyScored, status);
        _mockUserRateRepository.Verify(x => x.GetAllUserRates(), Times.Once);
        _mockUserRateRepository.Verify(x => x.AddUserRate(It.IsAny<UserRate>()), Times.Never);
    }

    /// <summary>
    /// Valid Conditions:
    /// Score in range 1,10
    /// No Existing score for that movie
    /// </summary>
    [Fact]
    public async Task AddScore_ValidCondition_ShouldReturnSuccess()
    {
        // Arrange
        ulong movieId = 1;
        ulong userId = 1;
        var score = 5;

        var userRates = new List<UserRate>()
        {
            new UserRate() { MovieId = movieId + 1, UserId = userId, Score = 3 }
        };
        _mockUserRateRepository.Setup(x => x.GetAllUserRates())
            .Returns(new TestAsyncEnumerable<UserRate>(userRates));
        _mockUserRateRepository.Setup(x => x.AddUserRate(It.IsAny<UserRate>()))
            .Returns((UserRate ur) => Task.FromResult(ur));

        // Act
        var status = await _sut.AddMovieScore(movieId, userId, score);

        // Arrange
        Assert.Equal(ScoreMovieResponse.Success, status);
        _mockUserRateRepository.Verify(x => x.GetAllUserRates(), Times.Once);
        _mockUserRateRepository.Verify(x => x.AddUserRate(It.IsAny<UserRate>()), Times.Once);
    }

    #endregion

    #region AddComment

    [Fact]
    public async Task AddComment_InvalidUserId_ShouldReturnFalse()
    {
        // Arrange
        ulong movieId = 1;
        ulong userId = 0;
        string comment = "test comment";
        _mockCommentRepository.Setup(x => x.AddComment(It.IsAny<Comment>()))
            .Returns((Comment c) => Task.FromResult(c));
        
        // Act
        var result = await _sut.AddMovieComment(movieId, userId, comment);
        
        // Assert
        Assert.False(result);
        _mockCommentRepository.Verify(x => x.AddComment(It.IsAny<Comment>()), Times.Never);
    }
    
    [Fact]
    public async Task AddComment_InvalidText_ShouldReturnFalse()
    {
        // Arrange
        ulong movieId = 1;
        ulong userId = 0;
        string comment = "";
        _mockCommentRepository.Setup(x => x.AddComment(It.IsAny<Comment>()))
            .Returns((Comment c) => Task.FromResult(c));
        
        // Act
        var result = await _sut.AddMovieComment(movieId, userId, comment);
        
        // Assert
        Assert.False(result);
        _mockCommentRepository.Verify(x => x.AddComment(It.IsAny<Comment>()), Times.Never);
    }

    #endregion
}