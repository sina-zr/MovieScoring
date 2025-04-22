using Microsoft.EntityFrameworkCore;
using Movies.Domain.Entities;
using Movies.Domain.Repositories;

namespace Movies.DataLayer.Repositories;

public class WatchlistRepository : IWatchlistRepository
{
    private readonly MovieContext _context;

    public WatchlistRepository(MovieContext context)
    {
        _context = context;
    }

    public IQueryable<Watchlist> GetAllWatchlists()
    {
        return _context.Watchlists.AsQueryable();
    }

    public async Task<Watchlist?> GetWatchlistById(ulong id)
    {
        return await _context.Watchlists.FindAsync(id);
    }

    public async Task<Watchlist> AddWatchlist(Watchlist watchlist)
    {
        await _context.Watchlists.AddAsync(watchlist);
        await _context.SaveChangesAsync();
        return watchlist;
    }

    public async Task<IEnumerable<Watchlist>> AddRangeWatchlists(IEnumerable<Watchlist> watchlists)
    {
        await _context.Watchlists.AddRangeAsync(watchlists);
        await _context.SaveChangesAsync();
        return watchlists;
    }

    public Watchlist UpdateWatchlist(Watchlist watchlist)
    {
        _context.Watchlists.Update(watchlist);
        _context.SaveChanges();
        return watchlist;
    }

    public IEnumerable<Watchlist> UpdateRangeWatchlists(IEnumerable<Watchlist> watchlists)
    {
        _context.Watchlists.UpdateRange(watchlists);
        _context.SaveChanges();
        return watchlists;
    }

    public bool DeleteWatchlist(Watchlist watchlist)
    {
        try
        {
            _context.Watchlists.Remove(watchlist);
            var changes = _context.SaveChanges();
            return changes > 0;
        }
        catch (Exception)
        {
            // Log exception here
            return false;
        }
    }

    public bool DeleteRangeWatchlists(IEnumerable<Watchlist> watchlists)
    {
        try
        {
            _context.Watchlists.RemoveRange(watchlists);
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