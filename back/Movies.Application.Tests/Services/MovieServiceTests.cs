using Moq;
using Movies.Application.Models.Movie;
using Movies.Application.Services.Implementations;
using Movies.Application.Services.Interfaces;
using Movies.Domain.Entities;
using Movies.Domain.Repositories;

namespace Movies.Application.Tests.Services;

public class MovieServiceTests
{
    private readonly Mock<IMovieRepository> _mockMovieRepository;
    private readonly Mock<IMovieGenreRepository> _mockMovieGenreRepository;
    
    private readonly IMovieService _movieService;

    public MovieServiceTests()
    {
        _mockMovieRepository = new Mock<IMovieRepository>();
        _mockMovieGenreRepository = new Mock<IMovieGenreRepository>();
        
        _movieService = new MovieService(_mockMovieRepository.Object, _mockMovieGenreRepository.Object);
    }

    #region GetMovies

    /// <summary>
    /// Test when there are no movies, the view model returned has no movies and zero pages.
    /// </summary>
    [Fact]
    public async Task GetMovies_NoMovies_ReturnsEmptyViewModel()
    {
        // Arrange
        var emptyMovies = new List<Movie>().AsQueryable();
        _mockMovieRepository.Setup(r => r.GetAllMovies())
            .Returns(new TestAsyncEnumerable<Movie>(emptyMovies));

        // Act
        var result = await _movieService.GetMovies(pageId: 1, pageSize: 10, filterTitle: null, genreId: null);

        // Assert
        Assert.Null(result.Movies);
        Assert.Equal(0, result.PagesCount);
    }

    /// <summary>
    /// Test that filtering by title returns only the movies whose titles contain the filter text.
    /// </summary>
    [Fact]
    public async Task GetMovies_FilterByTitle_ReturnsMatchingMovies()
    {
        // Arrange
        var movies = new List<Movie>
        {
            new Movie
            {
                Id = 1,
                Title = "Star Wars",
                Year = 1977,
                // Simulate a join to Genres (the inner Genre contains its Title)
                Genres = new List<MovieGenre>
                {
                    new MovieGenre { GenreId = 1, Genre = new Genre { Id = 1, Title = "Sci-Fi" } }
                },
                Rates = new List<UserRate> { new UserRate { MovieId = 1, Score = 8 } }
            },
            new Movie
            {
                Id = 2,
                Title = "The Godfather",
                Year = 1972,
                Genres = new List<MovieGenre>
                {
                    new MovieGenre { GenreId = 2, Genre = new Genre { Id = 2, Title = "Crime" } }
                },
                Rates = new List<UserRate> { new UserRate { MovieId = 2, Score = 10 } }
            }
        }.AsQueryable();

        _mockMovieRepository.Setup(r => r.GetAllMovies())
            .Returns(new TestAsyncEnumerable<Movie>(movies));

        // Act: filter for "Star" (should match "Star Wars")
        var result = await _movieService.GetMovies(pageId: 1, pageSize: 10, filterTitle: "Star", genreId: null);

        // Assert
        Assert.NotNull(result.Movies);
        Assert.Single(result.Movies);
        var movieVm = result.Movies.First();
        Assert.Equal("Star Wars", movieVm.Title);
        Assert.Equal(1977, movieVm.Year);
        Assert.Contains("Sci-Fi", movieVm.Genres!);

        // Since total movies matching filter = 1 and pageSize = 10, pages count should be 1.
        Assert.Equal(1, result.PagesCount);
    }

    /// <summary>
    /// Test that filtering by genre returns only movies that contain the genre.
    /// </summary>
    [Fact]
    public async Task GetMovies_FilterByGenre_ReturnsMoviesHavingThatGenre()
    {
        // Arrange
        var movies = new List<Movie>
        {
            new Movie
            {
                Id = 1,
                Title = "Movie A",
                Year = 2000,
                Genres = new List<MovieGenre>
                {
                    new MovieGenre { GenreId = 3, Genre = new Genre { Id = 3, Title = "Action" } }
                },
                Rates = new List<UserRate> { new UserRate { MovieId = 1, Score = 7 } }
            },
            new Movie
            {
                Id = 2,
                Title = "Movie B",
                Year = 2005,
                Genres = new List<MovieGenre>
                {
                    new MovieGenre { GenreId = 4, Genre = new Genre { Id = 4, Title = "Comedy" } }
                },
                Rates = new List<UserRate> { new UserRate { MovieId = 2, Score = 6 } }
            }
        }.AsQueryable();

        _mockMovieRepository.Setup(r => r.GetAllMovies())
            .Returns(new TestAsyncEnumerable<Movie>(movies));

        // Act: filter by GenreId 3 (Action)
        var result = await _movieService.GetMovies(pageId: 1, pageSize: 10, filterTitle: null, genreId: 3);

        // Assert
        Assert.NotNull(result.Movies);
        Assert.Single(result.Movies);
        var movieVm = result.Movies.First();
        Assert.Equal("Movie A", movieVm.Title);

        // Check that pages count is 1 (only one matching movie).
        Assert.Equal(1, result.PagesCount);
    }

    /// <summary>
    /// Test pagination: when pageSize is small and there are more movies than can fit on one page.
    /// </summary>
    [Fact]
    public async Task GetMovies_Pagination_WorksCorrectly()
    {
        // Arrange: Create 3 movies.
        var movies = new List<Movie>
        {
            new Movie
            {
                Id = 1,
                Title = "Movie 1",
                Year = 2001,
                Genres = new List<MovieGenre>
                {
                    new MovieGenre { GenreId = 1, Genre = new Genre { Id = 1, Title = "Genre 1" } }
                },
                Rates = new List<UserRate> { new UserRate { MovieId = 1, Score = 5 } }
            },
            new Movie
            {
                Id = 2,
                Title = "Movie 2",
                Year = 2002,
                Genres = new List<MovieGenre>
                {
                    new MovieGenre { GenreId = 2, Genre = new Genre { Id = 2, Title = "Genre 2" } }
                },
                Rates = new List<UserRate> { new UserRate { MovieId = 2, Score = 7 } }
            },
            new Movie
            {
                Id = 3,
                Title = "Movie 3",
                Year = 2003,
                Genres = new List<MovieGenre>
                {
                    new MovieGenre { GenreId = 3, Genre = new Genre { Id = 3, Title = "Genre 3" } }
                },
                Rates = new List<UserRate> { new UserRate { MovieId = 3, Score = 9 } }
            }
        }.AsQueryable();

        _mockMovieRepository.Setup(r => r.GetAllMovies())
            .Returns(new TestAsyncEnumerable<Movie>(movies));

        // Act: with pageSize=1, total movies=3, asking for page 2 should return the second movie.
        var result = await _movieService.GetMovies(pageId: 2, pageSize: 1, filterTitle: null, genreId: null);

        // Assert
        Assert.NotNull(result.Movies);
        Assert.Single(result.Movies);
        var movieVm = result.Movies.First();
        Assert.Equal("Movie 2", movieVm.Title);
        // Total pages should be 3
        Assert.Equal(3, result.PagesCount);
    }

    #endregion

    #region AddMovie

    [Fact]
    public async Task AddMovie_InvalidTitle_ReturnsFalse()
    {
        // Arrange - empty title
        var addMovieDto = new AddMovieDto
        {
            Title = "",
            Year = 2000
        };

        // Act
        var result = await _movieService.AddMovie(addMovieDto);

        // Assert
        Assert.Equal((ulong)0, result);
    }

    [Fact]
    public async Task AddMovie_YearTooEarly_ReturnsFalse()
    {
        // Arrange - year earlier than 1800
        var addMovieDto = new AddMovieDto
        {
            Title = "Test Movie",
            Year = 1700
        };

        // Act
        var result = await _movieService.AddMovie(addMovieDto);

        // Assert
        Assert.Equal((ulong)0, result);
    }

    [Fact]
    public async Task AddMovie_YearTooLate_ReturnsFalse()
    {
        // Arrange - year in the future
        var addMovieDto = new AddMovieDto
        {
            Title = "Test Movie",
            Year = DateTime.UtcNow.Year + 1
        };

        // Act
        var result = await _movieService.AddMovie(addMovieDto);

        // Assert
        Assert.Equal((ulong)0, result);
    }

    [Fact]
    public async Task AddMovie_LastIdLessThanOne_ReturnsFalse()
    {
        // Arrange
        var addMovieDto = new AddMovieDto
        {
            Title = "Test Movie",
            Year = 2000
        };

        // Simulate an invalid lastId (less than 1)
        _mockMovieRepository.Setup(r => r.FindLastId()).ReturnsAsync(0UL);

        // Act
        var result = await _movieService.AddMovie(addMovieDto);

        // Assert
        Assert.Equal((ulong)0, result);
    }

    [Fact]
    public async Task AddMovie_ValidMovie_ReturnsTrueAndCallsAddMovie()
    {
        // Arrange
        var addMovieDto = new AddMovieDto
        {
            Title = "Test Movie",
            Year = 2000
        };

        ulong lastId = 10;
        // Setup repository to return a valid lastId.
        _mockMovieRepository.Setup(r => r.FindLastId()).ReturnsAsync(lastId);
        // Setup AddMovie to simply return the movie that was passed in.
        _mockMovieRepository.Setup(r => r.AddMovie(It.IsAny<Movie>()))
            .ReturnsAsync((Movie m) => m);

        // Act
        var result = await _movieService.AddMovie(addMovieDto);

        // Assert
        Assert.True(result > 0);
        // Verify that repository.AddMovie was called with a Movie instance that has an Id equal to lastId + 1
        _mockMovieRepository.Verify(r => r.AddMovie(It.Is<Movie>(m =>
            m.Title == addMovieDto.Title &&
            m.Year == addMovieDto.Year &&
            m.Id == lastId + 1)), Times.Once);
    }

    #endregion

    private List<Movie> GetTestMovies()
    {
        return new List<Movie>
        {
            new Movie
            {
                Id = 1,
                Title = "Inception",
                Year = 2010,
                Genres = new List<MovieGenre>
                {
                    new MovieGenre { Genre = new Genre { Title = "Sci-Fi" }, GenreId = 1 }
                },
                Rates = new List<UserRate>
                {
                    new UserRate { MovieId = 1, Score = 4 },
                    new UserRate { MovieId = 1, Score = 5 }
                }
            },
            new Movie
            {
                Id = 2,
                Title = "Interstellar",
                Year = 2014,
                Genres = new List<MovieGenre>
                {
                    new MovieGenre { Genre = new Genre { Title = "Sci-Fi" }, GenreId = 1 }
                },
                Rates = new List<UserRate>
                {
                    new UserRate { MovieId = 1, Score = 5 }
                }
            },
            new Movie
            {
                Id = 3,
                Title = "The Prestige",
                Year = 2006,
                Genres = new List<MovieGenre>
                {
                    new MovieGenre { Genre = new Genre { Title = "Drama" }, GenreId = 2 }
                },
                Rates = new List<UserRate>()
            }
        };
    }
}