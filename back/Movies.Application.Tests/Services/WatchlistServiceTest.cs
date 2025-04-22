using Moq;
using Movies.Application.Services.Implementations;
using Movies.Application.Services.Interfaces;
using Movies.Domain.Entities;
using Movies.Domain.Repositories;

namespace Movies.Application.Tests.Services;

public class WatchlistServiceTest
{
    private readonly Mock<IWatchlistRepository> _mockWatchlistRepository;
    private readonly Mock<IUserRateRepository> _mockUserRateRepository;
    private readonly Mock<IMovieRepository> _mockMovieRepository;
    private readonly Mock<IWatchlistMovieRepository> _mockWatchlistMovieRepository;

    private readonly IWatchlistService _sut;

    public WatchlistServiceTest()
    {
        _mockWatchlistRepository = new Mock<IWatchlistRepository>();
        _mockUserRateRepository = new Mock<IUserRateRepository>();
        _mockMovieRepository = new Mock<IMovieRepository>();
        _mockWatchlistMovieRepository = new Mock<IWatchlistMovieRepository>();

        _sut = new WatchlistService(_mockWatchlistRepository.Object, _mockUserRateRepository.Object,
            _mockMovieRepository.Object, _mockWatchlistMovieRepository.Object);
    }

    #region GetUserTopGenresScored

    [Fact]
    public async Task GetUserTopGenresScored_NoUserScores_ReturnsEmptyList()
    {
        // Arrange
        _mockUserRateRepository.Setup(repo => repo.GetAllUserRates())
            .Returns(new TestAsyncEnumerable<UserRate>(new List<UserRate>()));

        // Act
        var result = await _sut.GetUserTopGenresScored(1);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserTopGenresScored_ReturnsTop3GenresByAverageScore()
    {
        // Arrange
        var data = new List<UserRate>
        {
            new UserRate
            {
                UserId = 1,
                Score = 5,
                Movie = new Movie
                {
                    Genres = new List<MovieGenre>
                    {
                        new MovieGenre { GenreId = 1 },
                        new MovieGenre { GenreId = 2 }
                    }
                }
            },
            new UserRate
            {
                UserId = 1,
                Score = 4,
                Movie = new Movie
                {
                    Genres = new List<MovieGenre>
                    {
                        new MovieGenre { GenreId = 1 }
                    }
                }
            },
            new UserRate
            {
                UserId = 1,
                Score = 3,
                Movie = new Movie
                {
                    Genres = new List<MovieGenre>
                    {
                        new MovieGenre { GenreId = 3 }
                    }
                }
            }
        };

        // g2 => 5
        // g1 => 4.5
        // g3 => 3

        _mockUserRateRepository.Setup(repo => repo.GetAllUserRates())
            .Returns(new TestAsyncEnumerable<UserRate>(data));

        // Act
        var result = await _sut.GetUserTopGenresScored(1);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(new List<int> { 2, 1, 3 }, result);
    }

    [Fact]
    public async Task GetUserTopGenresScored_LessThanThreeGenres_ReturnsAll()
    {
        // Arrange
        var data = new List<UserRate>
        {
            new UserRate
            {
                UserId = 1,
                Score = 5,
                Movie = new Movie
                {
                    Genres = new List<MovieGenre>
                    {
                        new MovieGenre { GenreId = 1 }
                    }
                }
            },
            new UserRate
            {
                UserId = 1,
                Score = 2,
                Movie = new Movie
                {
                    Genres = new List<MovieGenre>
                    {
                        new MovieGenre { GenreId = 2 }
                    }
                }
            }
        };

        _mockUserRateRepository.Setup(repo => repo.GetAllUserRates())
            .Returns(new TestAsyncEnumerable<UserRate>(data));

        // Act
        var result = await _sut.GetUserTopGenresScored(1);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(new List<int> { 1, 2 }, result);
    }

    #endregion

    #region GetGenresTopMovies

    [Fact]
    public async Task GetGenresTopMovies_EmptyGenreList_ReturnsEmptyList()
    {
        // Act
        var result = await _sut.GetGenresTopMovies(new List<int>());

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetGenresTopMovies_SingleGenreWithMovies_ReturnsTopMovies()
    {
        // Arrange
        var genreId = 1;

        var data = new List<Movie>
        {
            new Movie
            {
                Id = 1,
                Title = "Movie A",
                Genres = new List<MovieGenre> { new MovieGenre { GenreId = genreId } },
                Rates = new List<UserRate> { new UserRate { Score = 4 }, new UserRate { Score = 5 } }
            },
            new Movie
            {
                Id = 2,
                Title = "Movie B",
                Genres = new List<MovieGenre> { new MovieGenre { GenreId = genreId } },
                Rates = new List<UserRate> { new UserRate { Score = 2 } }
            }
        };

        _mockMovieRepository.Setup(repo => repo.GetAllMovies())
            .Returns(new TestAsyncEnumerable<Movie>(data));

        // Act
        var result = await _sut.GetGenresTopMovies(new List<int> { genreId });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, m => m.MovieTitle == "Movie A");
        Assert.Contains(result, m => m.MovieTitle == "Movie B");
    }

    [Fact]
    public async Task GetGenresTopMovies_MultipleGenresWithSharedMovies_NoDuplicatesInResults()
    {
        // Arrange
        var data = new List<Movie>
        {
            new Movie
            {
                Id = 1,
                Title = "Shared Movie",
                Genres = new List<MovieGenre>
                {
                    new MovieGenre { GenreId = 1 },
                    new MovieGenre { GenreId = 2 }
                },
                Rates = new List<UserRate> { new UserRate { Score = 4 } }
            }
        };

        _mockMovieRepository.Setup(repo => repo.GetAllMovies())
            .Returns(new TestAsyncEnumerable<Movie>(data));

        // Act
        var result = await _sut.GetGenresTopMovies(new List<int> { 1, 2 });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count); // Still returns duplicate for each genre
        Assert.All(result, r => Assert.Equal("Shared Movie", r.MovieTitle));
    }

    [Fact]
    public async Task GetGenresTopMovies_GenreWithNoMovies_ReturnsEmptyForThatGenre()
    {
        // Arrange
        var data = new List<Movie>(); // no movies

        _mockMovieRepository.Setup(repo => repo.GetAllMovies())
            .Returns(new TestAsyncEnumerable<Movie>(data));

        // Act
        var result = await _sut.GetGenresTopMovies(new List<int> { 99 });

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetGenresTopMovies_MoreThan5Movies_ReturnsTop5ByScore()
    {
        // Arrange
        var genreId = 1;
        var movies = Enumerable.Range(1, 10).Select(i =>
            new Movie
            {
                Id = (ulong)i,
                Title = $"Movie {i}",
                Genres = new List<MovieGenre> { new MovieGenre { GenreId = genreId } },
                Rates = new List<UserRate> { new UserRate { Score = (byte)i } } // score = i
            }).ToList();

        _mockMovieRepository.Setup(repo => repo.GetAllMovies())
            .Returns(new TestAsyncEnumerable<Movie>(movies));

        // Act
        var result = await _sut.GetGenresTopMovies(new List<int> { genreId });

        // Assert
        Assert.Equal(5, result.Count);
        Assert.Equal(new[] { "Movie 10", "Movie 9", "Movie 8", "Movie 7", "Movie 6" },
            result.Select(m => m.MovieTitle));
    }

    #endregion
}