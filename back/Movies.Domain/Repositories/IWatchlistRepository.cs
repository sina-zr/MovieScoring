using Movies.Domain.Entities;

namespace Movies.Domain.Repositories;

public interface IWatchlistRepository
{
        IQueryable<Watchlist> GetAllWatchlists();
        Task<Watchlist?> GetWatchlistById(ulong id);
        Task<Watchlist> AddWatchlist(Watchlist watchlist);
        Task<IEnumerable<Watchlist>> AddRangeWatchlists(IEnumerable<Watchlist> watchlists);
        Watchlist UpdateWatchlist(Watchlist watchlist);
        IEnumerable<Watchlist> UpdateRangeWatchlists(IEnumerable<Watchlist> watchlists);
        bool DeleteWatchlist(Watchlist watchlist);
        bool DeleteRangeWatchlists(IEnumerable<Watchlist> watchlists);
}