using Microsoft.EntityFrameworkCore;
using Movies.Domain.Entities;
using Movies.Domain.Repositories;

namespace Movies.DataLayer.Repositories;

public class WatchlistMovieRepository : IWatchlistMovieRepository
{
    private readonly MovieContext _context;

    public WatchlistMovieRepository(MovieContext context)
    {
        _context = context;
    }

    public IQueryable<WatchlistMovie> GetAllWatchlistMovies()
    {
        return _context.WatchlistsMovies.AsQueryable();
    }

    public async Task<WatchlistMovie?> GetWatchlistMovieById(ulong id)
    {
        return await _context.WatchlistsMovies.FindAsync(id);
    }

    public async Task<WatchlistMovie> AddWatchlistMovie(WatchlistMovie watchlistMovie)
    {
        await _context.WatchlistsMovies.AddAsync(watchlistMovie);
        await _context.SaveChangesAsync();
        return watchlistMovie;
    }

    public async Task<IEnumerable<WatchlistMovie>> AddRangeWatchlistMovies(IEnumerable<WatchlistMovie> watchlistMovies)
    {
        await _context.WatchlistsMovies.AddRangeAsync(watchlistMovies);
        await _context.SaveChangesAsync();
        return watchlistMovies;
    }

    public WatchlistMovie UpdateWatchlistMovie(WatchlistMovie watchlistMovie)
    {
        _context.WatchlistsMovies.Update(watchlistMovie);
        _context.SaveChanges();
        return watchlistMovie;
    }

    public IEnumerable<WatchlistMovie> UpdateRangeWatchlistMovies(IEnumerable<WatchlistMovie> watchlistMovies)
    {
        _context.WatchlistsMovies.UpdateRange(watchlistMovies);
        _context.SaveChanges();
        return watchlistMovies;
    }

    public bool DeleteWatchlistMovie(WatchlistMovie watchlistMovie)
    {
        try
        {
            _context.WatchlistsMovies.Remove(watchlistMovie);
            var changes = _context.SaveChanges();
            return changes > 0;
        }
        catch (Exception)
        {
            // Log exception here
            return false;
        }
    }

    public bool DeleteRangeWatchlistMovies(IEnumerable<WatchlistMovie> watchlistMovies)
    {
        try
        {
            _context.WatchlistsMovies.RemoveRange(watchlistMovies);
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