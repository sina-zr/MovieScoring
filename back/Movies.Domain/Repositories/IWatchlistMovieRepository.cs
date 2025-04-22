using Movies.Domain.Entities;

namespace Movies.Domain.Repositories;

public interface IWatchlistMovieRepository
{
    IQueryable<WatchlistMovie> GetAllWatchlistMovies();
    Task<WatchlistMovie?> GetWatchlistMovieById(ulong id);
    Task<WatchlistMovie> AddWatchlistMovie(WatchlistMovie watchlistMovie);
    Task<IEnumerable<WatchlistMovie>> AddRangeWatchlistMovies(IEnumerable<WatchlistMovie> watchlistMovies);
    WatchlistMovie UpdateWatchlistMovie(WatchlistMovie watchlistMovie);
    IEnumerable<WatchlistMovie> UpdateRangeWatchlistMovies(IEnumerable<WatchlistMovie> watchlistMovies);
    bool DeleteWatchlistMovie(WatchlistMovie watchlistMovie);
    bool DeleteRangeWatchlistMovies(IEnumerable<WatchlistMovie> watchlistMovies);
}