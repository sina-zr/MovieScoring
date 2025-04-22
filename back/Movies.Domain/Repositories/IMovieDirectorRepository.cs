using Movies.Domain.Entities;

namespace Movies.Domain.Repositories;

public interface IMovieDirectorRepository
{
    IQueryable<MovieDirector> GetAllMovieDirectors();
    Task<MovieDirector?> GetMovieDirectorById(ulong id);
    Task<MovieDirector> AddMovieDirector(MovieDirector movieDirector);
    Task<IEnumerable<MovieDirector>> AddRangeMovieDirectors(IEnumerable<MovieDirector> movieDirectors);
    MovieDirector UpdateMovieDirector(MovieDirector movieDirector);
    IEnumerable<MovieDirector> UpdateRangeMovieDirectors(IEnumerable<MovieDirector> movieDirectors);
    bool DeleteMovieDirector(MovieDirector movieDirector);
    bool DeleteRangeMovieDirectors(IEnumerable<MovieDirector> movieDirectors);
}