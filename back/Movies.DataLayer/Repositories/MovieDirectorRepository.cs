using Movies.Domain.Entities;
using Movies.Domain.Repositories;

namespace Movies.DataLayer.Repositories;

public class MovieDirectorRepository : IMovieDirectorRepository
{
    private readonly MovieContext _context;

    public MovieDirectorRepository(MovieContext context)
    {
        _context = context;
    }

    public IQueryable<MovieDirector> GetAllMovieDirectors()
    {
        return _context.MoviesDirectors.AsQueryable();
    }

    public async Task<MovieDirector?> GetMovieDirectorById(ulong id)
    {
        return await _context.MoviesDirectors.FindAsync(id);
    }

    public async Task<MovieDirector> AddMovieDirector(MovieDirector movieDirector)
    {
        await _context.MoviesDirectors.AddAsync(movieDirector);
        await _context.SaveChangesAsync();
        return movieDirector;
    }

    public async Task<IEnumerable<MovieDirector>> AddRangeMovieDirectors(IEnumerable<MovieDirector> movieDirectors)
    {
        await _context.MoviesDirectors.AddRangeAsync(movieDirectors);
        await _context.SaveChangesAsync();
        return movieDirectors;
    }

    public MovieDirector UpdateMovieDirector(MovieDirector movieDirector)
    {
        _context.MoviesDirectors.Update(movieDirector);
        _context.SaveChanges();
        return movieDirector;
    }

    public IEnumerable<MovieDirector> UpdateRangeMovieDirectors(IEnumerable<MovieDirector> movieDirectors)
    {
        _context.MoviesDirectors.UpdateRange(movieDirectors);
        _context.SaveChanges();
        return movieDirectors;
    }

    public bool DeleteMovieDirector(MovieDirector movieDirector)
    {
        try
        {
            _context.MoviesDirectors.Remove(movieDirector);
            var changes = _context.SaveChanges();
            return changes > 0;
        }
        catch (Exception)
        {
            // Log exception here
            return false;
        }
    }

    public bool DeleteRangeMovieDirectors(IEnumerable<MovieDirector> movieDirectors)
    {
        try
        {
            _context.MoviesDirectors.RemoveRange(movieDirectors);
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