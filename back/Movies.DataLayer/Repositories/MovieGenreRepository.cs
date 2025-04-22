using Movies.Domain.Entities;
using Movies.Domain.Repositories;

namespace Movies.DataLayer.Repositories;

public class MovieGenreRepository : IMovieGenreRepository
{
    private readonly MovieContext _context;

    public MovieGenreRepository(MovieContext context)
    {
        _context = context;
    }

    public IQueryable<MovieGenre> GetAllMovieGenres()
    {
        return _context.MoviesGenres.AsQueryable();
    }

    public async Task<MovieGenre?> GetMovieGenreById(ulong id)
    {
        return await _context.MoviesGenres.FindAsync(id);
    }

    public async Task<MovieGenre> AddMovieGenre(MovieGenre movieGenre)
    {
        await _context.MoviesGenres.AddAsync(movieGenre);
        await _context.SaveChangesAsync();
        return movieGenre;
    }

    public async Task<IEnumerable<MovieGenre>> AddRangeMovieGenres(IEnumerable<MovieGenre> movieGenres)
    {
        await _context.MoviesGenres.AddRangeAsync(movieGenres);
        await _context.SaveChangesAsync();
        return movieGenres;
    }

    public MovieGenre UpdateMovieGenre(MovieGenre movieGenre)
    {
        _context.MoviesGenres.Update(movieGenre);
        _context.SaveChanges();
        return movieGenre;
    }

    public IEnumerable<MovieGenre> UpdateRangeMovieGenres(IEnumerable<MovieGenre> movieGenres)
    {
        _context.MoviesGenres.UpdateRange(movieGenres);
        _context.SaveChanges();
        return movieGenres;
    }

    public bool DeleteMovieGenre(MovieGenre movieGenre)
    {
        try
        {
            _context.MoviesGenres.Remove(movieGenre);
            var changes = _context.SaveChanges();
            return changes > 0;
        }
        catch (Exception)
        {
            // Log exception here
            return false;
        }
    }

    public bool DeleteRangeMovieGenres(IEnumerable<MovieGenre> movieGenres)
    {
        try
        {
            _context.MoviesGenres.RemoveRange(movieGenres);
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