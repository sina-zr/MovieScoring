using Movies.Application.Models.Watchlist;
using Movies.Domain.Entities;

namespace Movies.Application.Services.Interfaces;

public interface IWatchlistService
{
    Task<List<WatchlistVm>?> GetUserWatchlists(ulong userId);
    Task<bool> CheckIfUserHasWatchlist(ulong userId);
    Task<List<int>> GetUserTopGenresScored(ulong userId);
    Task<List<MovieForWatchlist>> GetGenresTopMovies(List<int> genreIds);
    Task<List<MovieForWatchlist>> GetTopMovies();
    Task<bool> AddMovieToWatchlist(ulong userId, ulong movieId);
}