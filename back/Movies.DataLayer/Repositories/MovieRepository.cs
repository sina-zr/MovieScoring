using Microsoft.EntityFrameworkCore;
using Movies.Domain.Entities;
using Movies.Domain.Repositories;

namespace Movies.DataLayer.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly MovieContext _context;

    public MovieRepository(MovieContext context)
    {
        _context = context;
    }


    public IQueryable<Movie> GetAllMovies()
    {
        return _context.Movies.AsQueryable();
    }

    public async Task<Movie?> GetMovieById(ulong id)
    {
        return await _context.Movies.FindAsync(id);
    }

    public async Task<Movie> AddMovie(Movie movie)
    {
        await _context.Movies.AddAsync(movie);
        await _context.SaveChangesAsync();
        return movie;
    }

    public async Task<IEnumerable<Movie>> AddRangeMovies(IEnumerable<Movie> movies)
    {
        await _context.Movies.AddRangeAsync(movies);
        await _context.SaveChangesAsync();
        return movies;
    }

    public Movie UpdateMovie(Movie movie)
    {
        _context.Movies.Update(movie);
        _context.SaveChanges();
        return movie;
    }

    public IEnumerable<Movie> UpdateRangeMovies(IEnumerable<Movie> movies)
    {
        _context.Movies.UpdateRange(movies);
        _context.SaveChanges();
        return movies;
    }

    public bool DeleteMovie(Movie movie)
    {
        try
        {
            _context.Movies.Remove(movie);
            var changes = _context.SaveChanges();
            if (changes > 0)
            {
                return true;
            }

            return false;
        }
        catch (Exception e)
        {
            // Log
            return false;
        }
    }

    public bool DeleteRangeMovies(IEnumerable<Movie> movies)
    {
        try
        {
            _context.Movies.RemoveRange(movies);
            var changes = _context.SaveChanges();
            if (changes > 0)
            {
                return true;
            }

            return false;
        }
        catch (Exception e)
        {
            // Log
            return false;
        }
    }

    public async Task<ulong> FindLastId()
    {
        var lastId = await _context.Movies.MaxAsync(m => (ulong?)m.Id) ?? 0;
        return lastId;
    }
}