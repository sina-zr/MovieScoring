using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.ExtensionMethods;
using Movies.Application.Models.Watchlist;
using Movies.Application.Services.Interfaces;

namespace Movies.Api.Controllers;

[ApiController]
[Route("/api")]
public class WatchlistController : ControllerBase
{
    private readonly IWatchlistService _watchlistService;
    private readonly IMovieService _movieService;

    public WatchlistController(IWatchlistService watchlistService, IMovieService movieService)
    {
        _watchlistService = watchlistService;
        _movieService = movieService;
    }
    
    /// <summary>
    /// get list of movies of user watchlist
    /// </summary>
    /// <response code="204">No Content</response>
    /// <returns>list of  watchlist view model</returns>
    [HttpGet("[action]")]
    [Authorize]
    [ProducesResponseType<WatchlistVm>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserWatchlist()
    {
        // get userId from token
        var userId = User.GetUserId();
        
        var watchlist = await _watchlistService.GetUserWatchlists(userId);
        if (watchlist is null) return NoContent();
        return Ok(watchlist.First());
    }
    
    /// <summary>
    /// recommend a list of movies to user
    /// </summary>
    /// <response code="400">if user already has a watchlist</response>
    /// <returns>list of movies</returns>
    [HttpGet("[action]")]
    [Authorize]
    [ProducesResponseType<List<MovieForWatchlist>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> RecommendWatchlist()
    {
        var userId = User.GetUserId();
        
        // check if user already has watchlist
        if (await _watchlistService.CheckIfUserHasWatchlist(userId))
        {
            return BadRequest("User has already watchlist");
        }
        
        // check if user has score
        var topGenres = await _watchlistService.GetUserTopGenresScored(userId);
        List<MovieForWatchlist> watchlist = new();
        if (topGenres.Count > 0)
        {
            // get best movies of top genres
            watchlist = await _watchlistService.GetGenresTopMovies(topGenres);
        }
        else
        {
            // get best movies of all
            watchlist = await _watchlistService.GetTopMovies();
        }

        return Ok(watchlist);
    }

    /// <summary>
    /// Add a movie to a user watchlist, creates a watchlist if user does not have one
    /// </summary>
    /// <param name="movieId">ID of the movie to add to the watchlist</param>
    /// <returns></returns>
    /// <response code="200">The movie was successfully added to the user's watchlist.</response>
    /// <response code="400">Invalid request format or invalid movie ID provided.</response>
    /// <response code="404">The specified movie could not be found in the database.</response>
    /// <response code="500">Internal server error occurred while processing the request.</response>
    [HttpPost("[action]")]
    [Authorize]
    public async Task<IActionResult> AddMovieToWatchlist(ulong movieId)
    {
        var userId = User.GetUserId();
        
        var movie = await _movieService.GetMovieById(movieId);
        if (movie is null) return NotFound("Movie not found");

        if (await _watchlistService.AddMovieToWatchlist(userId, movieId))
            return Ok();
        else return StatusCode(500);
    }
    
}