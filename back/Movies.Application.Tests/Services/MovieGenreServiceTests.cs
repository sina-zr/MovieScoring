using Moq;
using Movies.Application.Services.Implementations;
using Movies.Application.Services.Interfaces;
using Movies.Domain.Entities;
using Movies.Domain.Repositories;

namespace Movies.Application.Tests.Services;

public class MovieGenreServiceTests
{
    private readonly IGenreManagementService _service;
    private readonly Mock<IGenreRepository> _mockGenreRepository;
    private readonly Mock<IMovieGenreRepository> _mockMovieGenreRepository;
    
    public MovieGenreServiceTests()
    {
        _mockGenreRepository = new Mock<IGenreRepository>();
        _mockMovieGenreRepository = new Mock<IMovieGenreRepository>();
        
        _service = new GenreManagementService(_mockGenreRepository.Object, _mockMovieGenreRepository.Object);
    }

    /// <summary>
    /// If no Genre Id is passed, it should return false
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task UpsertMovieGenre_EmptyGenreIds_ShouldReturnFalse()
    {
        // Arrange
        ulong movieId = 1;
        var genreIds = new List<int>(); // empty list

        // Act
        var result = await _service.UpsertMovieGenres(movieId, genreIds);

        // Assert
        Assert.False(result);

        // Verify that repository methods were not called
        _mockMovieGenreRepository.Verify(r => r.GetAllMovieGenres(), Times.Never);
        _mockMovieGenreRepository.Verify(r => r.AddRangeMovieGenres(It.IsAny<IEnumerable<MovieGenre>>()), Times.Never);
        _mockMovieGenreRepository.Verify(r => r.UpdateRangeMovieGenres(It.IsAny<IEnumerable<MovieGenre>>()),
            Times.Never);
    }

    /// <summary>
    /// If non of the Genre Ids exist for that movie, it should add all of them
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task UpsertMovieGenre_NoExistingGenres_ShouldAddAll()
    {
        // Arrange
        ulong param_movieId = 1;
        var param_genreIds = new List<int>() { 1, 2, 3 };

        var existingMovieGenres = new List<MovieGenre>() { new() { MovieId = 1, GenreId = 5 }, new() { MovieId = 1, GenreId = 6 } };

        _mockMovieGenreRepository.Setup(x => x.GetAllMovieGenres())
            .Returns(new TestAsyncEnumerable<MovieGenre>(existingMovieGenres));
        
        List<MovieGenre> capturedAddedGenres = null;
        _mockMovieGenreRepository.Setup(r => r.AddRangeMovieGenres(It.IsAny<IEnumerable<MovieGenre>>()))
            .ReturnsAsync((IEnumerable<MovieGenre> genres) =>
            {
                capturedAddedGenres = genres.ToList();
                return capturedAddedGenres;
            });
        
        
        // Act
        var result = await _service.UpsertMovieGenres(param_movieId, param_genreIds);
        
        // Verify that AddRangeMovieActors is called exactly once with 2 MovieActor objects.
        _mockMovieGenreRepository.Verify(r => r.AddRangeMovieGenres(It.IsAny<IEnumerable<MovieGenre>>()), Times.Once);
        Assert.NotNull(capturedAddedGenres);
        Assert.Equal(3, capturedAddedGenres.Count);
        // Check each new MovieActor is properly assigned.
        Assert.All(capturedAddedGenres, actor => Assert.Equal(param_movieId, actor.MovieId));
        var addedIds = capturedAddedGenres.Select(a => a.GenreId).ToList();
        Assert.Contains(2, addedIds);
        Assert.Contains(1, addedIds);
    }
}