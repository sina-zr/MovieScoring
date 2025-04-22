using Movies.Domain.Entities;

namespace Movies.Domain.Repositories;

public interface IMovieGenreRepository
{
    IQueryable<MovieGenre> GetAllMovieGenres();
    Task<MovieGenre?> GetMovieGenreById(ulong id);
    Task<MovieGenre> AddMovieGenre(MovieGenre movieGenre);
    Task<IEnumerable<MovieGenre>> AddRangeMovieGenres(IEnumerable<MovieGenre> movieGenres);
    MovieGenre UpdateMovieGenre(MovieGenre movieGenre);
    IEnumerable<MovieGenre> UpdateRangeMovieGenres(IEnumerable<MovieGenre> movieGenres);
    bool DeleteMovieGenre(MovieGenre movieGenre);
    bool DeleteRangeMovieGenres(IEnumerable<MovieGenre> movieGenres);
}