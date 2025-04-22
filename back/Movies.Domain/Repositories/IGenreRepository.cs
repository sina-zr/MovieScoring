using Movies.Domain.Entities;

namespace Movies.Domain.Repositories;

public interface IGenreRepository
{
    IQueryable<Genre> GetAllGenres();
    Task<Genre?> GetGenreById(int id);
    Task<Genre> AddGenre(Genre genre);
    Task<IEnumerable<Genre>> AddRangeGenres(IEnumerable<Genre> genres);
    Genre UpdateGenre(Genre genre);
    IEnumerable<Genre> UpdateRangeGenres(IEnumerable<Genre> genres);
    bool DeleteGenre(Genre genre);
    bool DeleteRangeGenres(IEnumerable<Genre> genres);
}