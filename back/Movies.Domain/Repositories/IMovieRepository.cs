using Movies.Domain.Entities;

namespace Movies.Domain.Repositories;

public interface IMovieRepository
{
    IQueryable<Movie> GetAllMovies();
    Task<Movie?> GetMovieById(ulong id);
    Task<Movie> AddMovie(Movie movie);
    Task<IEnumerable<Movie>> AddRangeMovies(IEnumerable<Movie> movies);
    Movie UpdateMovie(Movie movie);
    IEnumerable<Movie> UpdateRangeMovies(IEnumerable<Movie> movies);
    bool DeleteMovie(Movie movie);
    bool DeleteRangeMovies(IEnumerable<Movie> movies);
    Task<ulong> FindLastId();
}