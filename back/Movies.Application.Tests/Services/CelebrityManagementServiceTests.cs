using Moq;
using Movies.Application.Models.Celebrity;
using Movies.Application.Services.Implementations;
using Movies.Application.Services.Interfaces;
using Movies.Domain.Entities;
using Movies.Domain.Repositories;

namespace Movies.Application.Tests.Services;

public class CelebrityManagementServiceTests
{
    private readonly Mock<ICinemaPeopleRepository> _mockCinemaPeopleRepository;
    private readonly Mock<IMovieActorRepository> _mockMovieActorRepository;
    private readonly Mock<IMovieDirectorRepository> _mockMovieDirectorRepository;

    private readonly ICelebrityManagementService _service;

    public CelebrityManagementServiceTests()
    {
        _mockCinemaPeopleRepository = new Mock<ICinemaPeopleRepository>();
        _mockMovieActorRepository = new Mock<IMovieActorRepository>();
        _mockMovieDirectorRepository = new Mock<IMovieDirectorRepository>();

        _service = new CelebrityManagementService(_mockCinemaPeopleRepository.Object,
            _mockMovieActorRepository.Object, _mockMovieDirectorRepository.Object);
    }

    #region GetAllCelebrities

    /// <summary>
    /// Test what happens if the Cinema People is empty
    /// </summary>
    [Fact]
    public async Task GetAllCelebrities_NoCinemaPeople_ShouldReturnEmptyVm()
    {
        // Arrange
        var emptyCinemaPeople = new List<CinemaPeople>().AsQueryable();
        _mockCinemaPeopleRepository.Setup(r => r.GetAllCinemaPeople())
            .Returns(new TestAsyncEnumerable<CinemaPeople>(emptyCinemaPeople));

        // Act
        var result = await _service.GetAllCelebrities(pageId: 1, pageSize: 10, filterName: null);

        // Assert
        Assert.Null(result.Celebrities);
        Assert.Equal(0, result.PagesCount);
    }

    /// <summary>
    /// Tests that filtering by a celebrity's name returns only matching results.
    /// </summary>
    [Fact]
    public async Task GetAllCelebrities_FilterByName_ReturnsMatchingCelebrity()
    {
        // Arrange: set up two celebrities
        var celebrities = new List<CinemaPeople>
        {
            new CinemaPeople { Id = 1, FullName = "John Doe", BirthYear = 1980 },
            new CinemaPeople { Id = 2, FullName = "Jane Smith", BirthYear = 1990 }
        }.AsQueryable();

        var asyncCelebrities = new TestAsyncEnumerable<CinemaPeople>(celebrities);
        _mockCinemaPeopleRepository.Setup(x => x.GetAllCinemaPeople())
            .Returns(asyncCelebrities);

        // Act: Filter for "John"
        var result = await _service.GetAllCelebrities(pageId: 1, pageSize: 10, filterName: "John");

        // Assert
        Assert.NotNull(result.Celebrities);
        var list = result.Celebrities.ToList();
        Assert.Single(list);
        var celebVm = list.First();
        Assert.Equal("John Doe", celebVm.FullName);
        Assert.Equal(1980, celebVm.BirthYear);
        Assert.Equal((ulong)1, celebVm.CelebrityId);
        // With only one record matching and pageSize of 10, PagesCount should be 1.
        Assert.Equal(1, result.PagesCount);
    }

    /// <summary>
    /// Tests pagination works correctly: with a page size of 1 on 3 records, page 2 should return the second celebrity.
    /// </summary>
    [Fact]
    public async Task GetAllCelebrities_Pagination_ReturnsCorrectPage()
    {
        // Arrange: Create 3 celebrity records.
        var celebrities = new List<CinemaPeople>
        {
            new CinemaPeople { Id = 1, FullName = "Celebrity 1", BirthYear = 1975 },
            new CinemaPeople { Id = 2, FullName = "Celebrity 2", BirthYear = 1980 },
            new CinemaPeople { Id = 3, FullName = "Celebrity 3", BirthYear = 1990 }
        }.AsQueryable();

        var asyncCelebrities = new TestAsyncEnumerable<CinemaPeople>(celebrities);
        _mockCinemaPeopleRepository.Setup(x => x.GetAllCinemaPeople())
            .Returns(asyncCelebrities);

        // Act: request page 2 with a page size of 1; total pages should be 3
        var result = await _service.GetAllCelebrities(pageId: 2, pageSize: 1, filterName: null);

        // Assert
        Assert.NotNull(result.Celebrities);
        var list = result.Celebrities.ToList();
        Assert.Single(list);
        var celebVm = list.First();
        Assert.Equal("Celebrity 2", celebVm.FullName);
        // Total pages: (3 records / 1 per page) = 3 pages.
        Assert.Equal(3, result.PagesCount);
    }

    #endregion

    #region AddCelebrity

    [Fact]
    public async Task AddCelebrity_ShouldAddCelebritySuccessfully()
    {
        // Arrange
        string celebrityName = "John Snow";
        int celbrityBirthYear = 1980;

        // Setup repository to return a valid lastId.
        _mockCinemaPeopleRepository.Setup(r => r.GetLastCinemaPeopleId()).ReturnsAsync((ulong)1244);
        _mockCinemaPeopleRepository.Setup(r => r.AddCinemaPeople(It.IsAny<CinemaPeople>()))
            .ReturnsAsync(It.IsAny<CinemaPeople>());

        // Act
        var result = await _service.AddCinemaPeople(celebrityName, celbrityBirthYear);

        Assert.True(result);
    }

    #endregion

    #region UpsertMovieActors

    /// <summary>
    /// If the list of actor IDs is empty, the method should return false.
    /// </summary>
    [Fact]
    public async Task UpsertMovieActors_EmptyActorIds_ReturnsFalse()
    {
        // Arrange
        ulong movieId = 1;
        var actorIds = new List<ulong>(); // empty list

        // Act
        var result = await _service.UpsertMovieActors(movieId, actorIds);

        // Assert
        Assert.False(result);

        // Verify that repository methods were not called
        _mockMovieActorRepository.Verify(r => r.GetAllMovieActors(), Times.Never);
        _mockMovieActorRepository.Verify(r => r.AddRangeMovieActors(It.IsAny<IEnumerable<MovieActor>>()), Times.Never);
        _mockMovieActorRepository.Verify(r => r.UpdateRangeMovieActors(It.IsAny<IEnumerable<MovieActor>>()),
            Times.Never);
    }

    /// <summary>
    /// When there are no existing movie actor records, every actorId should be added.
    /// </summary>
    [Fact]
    public async Task UpsertMovieActors_NoExistingActors_AddsAllNewActors()
    {
        // Arrange
        ulong movieId = 1;
        var actorIds = new List<ulong> { 10, 20 };

        // Set up repository so that GetAllMovieActors returns an empty list.
        var emptyList = new List<MovieActor>().AsQueryable();
        _mockMovieActorRepository.Setup(r => r.GetAllMovieActors())
            .Returns(new TestAsyncEnumerable<MovieActor>(emptyList));

        List<MovieActor> capturedAddedActors = null;
        _mockMovieActorRepository.Setup(r => r.AddRangeMovieActors(It.IsAny<IEnumerable<MovieActor>>()))
            .ReturnsAsync((IEnumerable<MovieActor> actors) =>
            {
                capturedAddedActors = actors.ToList();
                return capturedAddedActors;
            });

        // Act
        var result = await _service.UpsertMovieActors(movieId, actorIds);

        // Assert
        Assert.True(result);
        // Expect that no actors are removed (update call with an empty list)
        _mockMovieActorRepository.Verify(r => r.UpdateRangeMovieActors(It.Is<IEnumerable<MovieActor>>(l => !l.Any())),
            Times.Once);

        // Verify that AddRangeMovieActors is called exactly once with 2 MovieActor objects.
        _mockMovieActorRepository.Verify(r => r.AddRangeMovieActors(It.IsAny<IEnumerable<MovieActor>>()), Times.Once);
        Assert.NotNull(capturedAddedActors);
        Assert.Equal(2, capturedAddedActors.Count);
        // Check each new MovieActor is properly assigned.
        Assert.All(capturedAddedActors, actor => Assert.Equal(movieId, actor.MovieId));
        var addedIds = capturedAddedActors.Select(a => a.CinemaPeopleId).ToList();
        Assert.Contains((ulong)10, addedIds);
        Assert.Contains((ulong)20, addedIds);
    }

    /// <summary>
    /// When some movie actor records already exist, they should be kept
    /// and only the missing actor IDs should be added.
    /// </summary>
    [Fact]
    public async Task UpsertMovieActors_SomeExistingActors_AddsMissingOnes()
    {
        // Arrange
        ulong movieId = 1;
        var actorIds = new List<ulong> { 10, 20, 30 };

        // Simulate an existing MovieActor for actorId 10.
        var existingMovieActors = new List<MovieActor>
        {
            new MovieActor { MovieId = movieId, CinemaPeopleId = 10, IsDelete = false }
        }.AsQueryable();

        _mockMovieActorRepository.Setup(r => r.GetAllMovieActors())
            .Returns(new TestAsyncEnumerable<MovieActor>(existingMovieActors));

        List<MovieActor> capturedAddedActors = null;
        _mockMovieActorRepository.Setup(r => r.AddRangeMovieActors(It.IsAny<IEnumerable<MovieActor>>()))
            .ReturnsAsync((IEnumerable<MovieActor> actors) =>
            {
                capturedAddedActors = actors.ToList();
                return capturedAddedActors;
            });

        // Act
        var result = await _service.UpsertMovieActors(movieId, actorIds);

        // Assert
        Assert.True(result);
        // Since the repository query is filtered by actorIds in the Where clause,
        // no removals should be detected.
        _mockMovieActorRepository.Verify(r => r.UpdateRangeMovieActors(It.Is<IEnumerable<MovieActor>>(l => !l.Any())),
            Times.Once);

        // Expect that AddRangeMovieActors is called for the missing actor IDs (20 and 30).
        _mockMovieActorRepository.Verify(r => r.AddRangeMovieActors(It.IsAny<IEnumerable<MovieActor>>()), Times.Once);
        Assert.NotNull(capturedAddedActors);
        Assert.Equal(2, capturedAddedActors.Count);
        Assert.All(capturedAddedActors, actor => Assert.Equal(movieId, actor.MovieId));
        var addedIds = capturedAddedActors.Select(a => a.CinemaPeopleId).ToList();
        Assert.DoesNotContain((ulong)10, addedIds); // Already exists so should not be added.
        Assert.Contains((ulong)20, addedIds);
        Assert.Contains((ulong)30, addedIds);
    }

    #endregion

    #region UpsertMovieDirectors

    [Fact]
        public async Task UpsertMovieDirectors_EmptyDirectorIds_ReturnsFalse()
        {
            // Arrange
            ulong movieId = 1;
            var directorIds = new List<ulong>(); // empty list

            // Act
            var result = await _service.UpsertMovieDirectors(movieId, directorIds);

            // Assert
            Assert.False(result);
            // Verify that GetAllMovieDirectors was never called.
            _mockMovieDirectorRepository.Verify(r => r.GetAllMovieDirectors(), Times.Never);
            _mockMovieDirectorRepository.Verify(r => r.AddRangeMovieDirectors(It.IsAny<IEnumerable<MovieDirector>>()), Times.Never);
            _mockMovieDirectorRepository.Verify(r => r.UpdateRangeMovieDirectors(It.IsAny<IEnumerable<MovieDirector>>()), Times.Never);
        }

        [Fact]
        public async Task UpsertMovieDirectors_NoExistingDirectors_AddsAllNewDirectors()
        {
            // Arrange
            ulong movieId = 1;
            var directorIds = new List<ulong> { 100, 200 };

            // Set up repository to return an empty collection.
            var emptyDirectors = new List<MovieDirector>().AsQueryable();
            _mockMovieDirectorRepository.Setup(r => r.GetAllMovieDirectors())
                .Returns(new TestAsyncEnumerable<MovieDirector>(emptyDirectors));

            List<MovieDirector> capturedAddedDirectors = null;
            _mockMovieDirectorRepository.Setup(r => r.AddRangeMovieDirectors(It.IsAny<IEnumerable<MovieDirector>>()))
                .ReturnsAsync((IEnumerable<MovieDirector> directors) =>
                {
                    capturedAddedDirectors = directors.ToList();
                    return capturedAddedDirectors;
                });

            // Act
            var result = await _service.UpsertMovieDirectors(movieId, directorIds);

            // Assert
            Assert.True(result);
            // Since no directors existed, UpdateRange should be called with an empty collection.
            _mockMovieDirectorRepository.Verify(r => r.UpdateRangeMovieDirectors(It.Is<IEnumerable<MovieDirector>>(l => !l.Any())), Times.Once);

            // Verify that AddRangeMovieDirectors is called once with 2 new MovieDirector items.
            _mockMovieDirectorRepository.Verify(r => r.AddRangeMovieDirectors(It.IsAny<IEnumerable<MovieDirector>>()), Times.Once);
            Assert.NotNull(capturedAddedDirectors);
            Assert.Equal(2, capturedAddedDirectors.Count);
            Assert.All(capturedAddedDirectors, d => Assert.Equal(movieId, d.MovieId));
            var addedIds = capturedAddedDirectors.Select(d => d.CinemaPeopleId).ToList();
            Assert.Contains((ulong)100, addedIds);
            Assert.Contains((ulong)200, addedIds);
        }

        [Fact]
        public async Task UpsertMovieDirectors_SomeExistingDirectors_AddsMissingDirectors()
        {
            // Arrange
            ulong movieId = 1;
            var directorIds = new List<ulong> { 100, 200, 300 };

            // Simulate an existing MovieDirector for directorId 100.
            var existingDirectors = new List<MovieDirector>
            {
                new MovieDirector { MovieId = movieId, CinemaPeopleId = 100, IsDelete = false }
            }.AsQueryable();

            _mockMovieDirectorRepository.Setup(r => r.GetAllMovieDirectors())
                .Returns(new TestAsyncEnumerable<MovieDirector>(existingDirectors));

            List<MovieDirector> capturedAddedDirectors = null;
            _mockMovieDirectorRepository.Setup(r => r.AddRangeMovieDirectors(It.IsAny<IEnumerable<MovieDirector>>()))
                .ReturnsAsync((IEnumerable<MovieDirector> directors) =>
                {
                    capturedAddedDirectors = directors.ToList();
                    return capturedAddedDirectors;
                });

            // Act
            var result = await _service.UpsertMovieDirectors(movieId, directorIds);

            // Assert
            Assert.True(result);
            // Since the repository query filters only directors in the provided list,
            // every returned director is in directorIds so directorsToRemove will be empty.
            _mockMovieDirectorRepository.Verify(r => r.UpdateRangeMovieDirectors(It.Is<IEnumerable<MovieDirector>>(l => !l.Any())), Times.Once);

            // The missing director IDs (200 and 300) should be added.
            _mockMovieDirectorRepository.Verify(r => r.AddRangeMovieDirectors(It.IsAny<IEnumerable<MovieDirector>>()), Times.Once);
            Assert.NotNull(capturedAddedDirectors);
            Assert.Equal(2, capturedAddedDirectors.Count);
            Assert.All(capturedAddedDirectors, d => Assert.Equal(movieId, d.MovieId));
            var addedIds = capturedAddedDirectors.Select(d => d.CinemaPeopleId).ToList();
            Assert.DoesNotContain((ulong)100, addedIds); // Already exists.
            Assert.Contains((ulong)200, addedIds);
            Assert.Contains((ulong)300, addedIds);
        }

    #endregion
}