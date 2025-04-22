using Microsoft.EntityFrameworkCore;
using Movies.Application.Models.Watchlist;
using Movies.Application.Services.Interfaces;
using Movies.Domain.Entities;
using Movies.Domain.Repositories;

namespace Movies.Application.Services.Implementations;

public class WatchlistService : IWatchlistService
{
    private readonly IWatchlistRepository _watchlistRepository;
    private readonly IUserRateRepository _userRateRepository;
    private readonly IMovieRepository _movieRepository;
    private readonly IWatchlistMovieRepository _watchlistMovieRepository;

    public WatchlistService(IWatchlistRepository watchlistRepository,
        IUserRateRepository userRateRepository, IMovieRepository movieRepository, IWatchlistMovieRepository watchlistMovieRepository)
    {
        _watchlistRepository = watchlistRepository;
        _userRateRepository = userRateRepository;
        _movieRepository = movieRepository;
        _watchlistMovieRepository = watchlistMovieRepository;
    }

    public async Task<List<WatchlistVm>?> GetUserWatchlists(ulong userId)
    {
        var watchlists = await _watchlistRepository.GetAllWatchlists()
            .Include(w => w.WatchlistMovies).ThenInclude(wm => wm.Movie)
            .Where(w => w.UserId == userId).ToListAsync();

        if (!watchlists.Any())
            return null;

        return watchlists.Select(w => new WatchlistVm
        {
            Movies = w.WatchlistMovies.Select(wm => new MovieForWatchlist()
            {
                MovieId = wm.MovieId,
                MovieTitle = wm.Movie.Title,
            }).ToList(),
            WatchlistName = w.WatchListName
        }).ToList();
    }

    public async Task<bool> CheckIfUserHasWatchlist(ulong userId)
    {
        return await _watchlistRepository.GetAllWatchlists()
            .AnyAsync(w => w.UserId == userId);
    }

    public async Task<List<int>> GetUserTopGenresScored(ulong userId)
    {
        var userScores = await _userRateRepository.GetAllUserRates()
            .Include(ur => ur.Movie)
            .ThenInclude(m => m.Genres)
            .Where(ur => ur.UserId == userId)
            .ToListAsync();

        if (!userScores.Any())
        {
            return new List<int>();
        }

        // Dictionary to map GenreId => List of user scores
        var genreScores = new Dictionary<int, List<int>>();

        foreach (var userRate in userScores)
        {
            var score = userRate.Score;
            var genres = userRate.Movie.Genres.Select(g => g.GenreId).ToList();

            foreach (var genreId in genres)
            {
                if (!genreScores.ContainsKey(genreId))
                {
                    genreScores[genreId] = new List<int>();
                }

                genreScores[genreId].Add(score);
            }
        }

        // Calculate average score per genre
        // select top 3
        var topGenres = genreScores
            .Select(g => new
            {
                GenreId = g.Key,
                Average = g.Value.Average()
            })
            .OrderByDescending(g => g.Average)
            .Take(3)
            .Select(g => g.GenreId)
            .ToList();

        return topGenres;
    }

    public async Task<List<MovieForWatchlist>> GetGenresTopMovies(List<int> genreIds)
    {
        var watchlistMovies = new List<MovieForWatchlist>();

        foreach (var genreId in genreIds)
        {
            var movies = await _movieRepository.GetAllMovies()
                .Include(m => m.Genres)
                .Include(m => m.Rates)
                .Where(m => m.Genres.Any(g => g.GenreId == genreId))
                .Select(m => new
                {
                    movieId = m.Id,
                    movieTitle = m.Title,
                    avScore = m.Rates.Average(r => r.Score),
                })
                .OrderByDescending(m => m.avScore)
                .Take(5).Select(m => new MovieForWatchlist()
                {
                    MovieId = m.movieId,
                    MovieTitle = m.movieTitle,
                }).ToListAsync();

            watchlistMovies.AddRange(movies);
        }

        return watchlistMovies;
    }

    public async Task<List<MovieForWatchlist>> GetTopMovies()
    {
        return await _movieRepository.GetAllMovies()
            .Include(m => m.Rates)
            .OrderByDescending(m => m.Rates.Sum(r => r.Score))
            .Take(10)
            .Select(m => new MovieForWatchlist()
            {
                MovieId = m.Id,
                MovieTitle = m.Title,
            }).ToListAsync();
    }

    public async Task<bool> AddMovieToWatchlist(ulong userId, ulong movieId)
    {
        if (userId < 1 || movieId < 1)
        {
            return false;
        }

        try
        {
            var watchlist = await _watchlistRepository.GetAllWatchlists().FirstOrDefaultAsync(w => w.UserId == userId);
            if (watchlist == null)
            {
                watchlist = await _watchlistRepository.AddWatchlist(new Watchlist()
                {
                    UserId = userId,
                    WatchListName = $"Watchlist for user {userId}",
                    CreateDate = DateTime.Now,
                    IsDelete = false
                });
            }

            var newWatchlistMovie = new WatchlistMovie()
            {
                MovieId = movieId,
                WatchlistId = watchlist.Id,
            };
            await _watchlistMovieRepository.AddWatchlistMovie(newWatchlistMovie);
            
            return true;
        }
        catch (Exception e)
        {
            // log
            return false;
        }
    }
}