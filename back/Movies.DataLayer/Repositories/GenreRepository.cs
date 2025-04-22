using Movies.Domain.Entities;
using Movies.Domain.Repositories;

namespace Movies.DataLayer.Repositories;

public class GenreRepository : IGenreRepository
{
    private readonly MovieContext _context;

    public GenreRepository(MovieContext context)
    {
        _context = context;
    }

    public IQueryable<Genre> GetAllGenres()
    {
        return _context.Genres.AsQueryable();
    }

    public async Task<Genre?> GetGenreById(int id)
    {
        return await _context.Genres.FindAsync(id);
    }

    public async Task<Genre> AddGenre(Genre genre)
    {
        await _context.Genres.AddAsync(genre);
        await _context.SaveChangesAsync();
        return genre;
    }

    public async Task<IEnumerable<Genre>> AddRangeGenres(IEnumerable<Genre> genres)
    {
        await _context.Genres.AddRangeAsync(genres);
        await _context.SaveChangesAsync();
        return genres;
    }

    public Genre UpdateGenre(Genre genre)
    {
        _context.Genres.Update(genre);
        _context.SaveChanges();
        return genre;
    }

    public IEnumerable<Genre> UpdateRangeGenres(IEnumerable<Genre> genres)
    {
        _context.Genres.UpdateRange(genres);
        _context.SaveChanges();
        return genres;
    }

    public bool DeleteGenre(Genre genre)
    {
        try
        {
            _context.Genres.Remove(genre);
            var changes = _context.SaveChanges();
            return changes > 0;
        }
        catch (Exception)
        {
            // Log exception here
            return false;
        }
    }

    public bool DeleteRangeGenres(IEnumerable<Genre> genres)
    {
        try
        {
            _context.Genres.RemoveRange(genres);
            var changes = _context.SaveChanges();
            return changes > 0;
        }
        catch (Exception)
        {
            // Log exception here
            return false;
        }
    }
}