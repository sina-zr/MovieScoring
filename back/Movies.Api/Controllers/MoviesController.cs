using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Movies.Api.ExtensionMethods;
using Movies.Api.Models;
using Movies.Api.Models.Movie;
using Movies.Application.Models.Celebrity;
using Movies.Application.Models.Genre;
using Movies.Application.Models.Movie;
using Movies.Application.Services.Interfaces;
using Movies.Domain.Entities;

namespace Movies.Api.Controllers;

[ApiController]
[Route("/api")]
[ApiExplorerSettings(IgnoreApi = false)]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly IMovieReactionService _movieReactionService;
    private readonly IGenreManagementService _genreManagementService;
    private readonly ICelebrityManagementService _celebrityManagementService;

    public MoviesController(IMovieService movieService, IMovieReactionService movieReactionService,
        IGenreManagementService genreManagementService, ICelebrityManagementService celebrityManagementService)
    {
        _movieService = movieService;
        _movieReactionService = movieReactionService;
        _genreManagementService = genreManagementService;
        _celebrityManagementService = celebrityManagementService;
    }

    /// <summary>
    /// Get a list of movies, filterable
    /// </summary>
    /// <param name="moviesParams"></param>
    /// <response code="200">MovieListVm</response>
    /// <response code="204">No Content</response>
    /// <returns></returns>
    [HttpGet("[action]")]
    [ProducesResponseType<MovieListVm>(StatusCodes.Status200OK)]
    [EnableRateLimiting("FixedWindow")]
    public async Task<IActionResult> GetMovies([FromQuery] MoviesParams moviesParams)
    {
        var result = await _movieService.GetMovies(moviesParams.PageId,
            moviesParams.PageSize, moviesParams.TitleFilter, moviesParams.GenreId);

        if (result.Movies == null || !result.Movies.Any())
        {
            return NoContent();
        }
        
        return Ok(new MoviesListVm()
        {
            Movies = result.Movies,
            PageId = moviesParams.PageId,
            PagesCount = result.PagesCount,
        });
    }
    
    /// <summary>
    /// Returns all Details of a movie
    /// </summary>
    /// <param name="id"></param>
    /// <response code="200">MovieDetailsVm</response>
    /// <response code="404">Movie Not Found</response>
    /// <returns>MovieDetailsVm</returns>
    [HttpGet("[action]/{id}")]
    [ProducesResponseType<MovieDetailsVm>(StatusCodes.Status200OK)]
    [EnableRateLimiting("FixedWindow")]
    public async Task<IActionResult> GetMovies(ulong id)
    {
        var movie = await _movieService.GetMovieDetails(id);
        if (movie == null)
        {
            return NotFound("Movie not found");
        }
        
        return Ok(movie);
    }
    
    /// <summary>
    /// Score a movie by user, range 1 to 10
    /// </summary>
    /// <param name="scoreDto"></param>
    /// <response code="400">invalid score</response>
    /// <response code="400">this Movie Already scored by this user</response>
    /// <response code="404">Movie not found</response>
    /// <response code="500">Server Error</response>
    /// <returns>Http Status Codes</returns>
    [HttpPost("[action]")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ScoreMovie([FromBody] ScoreMovieDto scoreDto)
    {
        // check to find movie
        var movie = await _movieService.GetMovieById(scoreDto.MovieId);
        if (movie == null)
        {
            return NotFound("Movie not found");
        }
        
        // get userId from Token
        var userId = User.GetUserId();
        
        // try to add score
        var result = await _movieReactionService.AddMovieScore(movie.Id, userId, scoreDto.Score);
        switch (result)
        {
            case ScoreMovieResponse.AlreadyScored:
                return BadRequest("Movie already scored");
            case ScoreMovieResponse.InvalidScore:
                return BadRequest("Invalid score");
            case ScoreMovieResponse.Success:
                return Ok();
            default:
                return StatusCode(500);
        }
    }
    
    /// <summary>
    /// Get the score of a user for a movie
    /// </summary>
    /// <param name="movieId"></param>
    /// <response code="404">movie not found</response>
    /// <response code="500">Server error</response>
    /// <returns>score from 1 to 10, or 0 if not scored yet</returns>
    [HttpGet("[action]")]
    [Authorize]
    [ProducesResponseType<int>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserMovieScore(ulong movieId)
    {
        var userId = User.GetUserId();
        
        var movie = await _movieService.GetMovieById(movieId);
        if (movie == null)
        {
            return NotFound("Movie not found");
        }
        
        var rate = await _movieReactionService.GetUserMovieScore(userId, movieId);
        return Ok(rate?.Score ?? 0);
    }
    
    /// <summary>
    /// Add a comment to a movie
    /// </summary>
    /// <param name="commentDto"></param>
    /// <returns>Http Status Codes</returns>
    [HttpPost("[action]")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CommentMovie([FromBody] AddCommentDto commentDto)
    {
        // check if movie exists
        var movie = await _movieService.GetMovieById(commentDto.MovieId);
        if (movie == null)
        {
            return NotFound("Movie not found");
        }
        
        // get userId
        var userId = User.GetUserId();
        
        // call add service
        if (await _movieReactionService.AddMovieComment(movie.Id, userId, commentDto.Text))
        {
            return Ok();
        }

        return StatusCode(500);
    }

    /// <summary>
    /// get a list of all genres
    /// </summary>
    /// <returns></returns>
    [HttpGet("[action]")]
    [EnableRateLimiting("FixedWindow")]
    [ProducesResponseType<List<GenreVm>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllGenres()
    {
        return Ok(await _genreManagementService.GetAllGenres());
    }
    
    /// <summary>
    /// Get a list of celebrities
    /// </summary>
    /// <param name="pageId"></param>
    /// <param name="pageSize"></param>
    /// <param name="filterName">filter celebrity full name</param>
    /// <returns>list of celebrities</returns>
    [HttpGet("[action]")]
    [ProducesResponseType<CelebrityListVm>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetAllCelebrities(int pageId = 1, int pageSize = 10, string? filterName = null)
    {
        var result = await _celebrityManagementService.GetAllCelebrities(pageId, pageSize, filterName);
        if (result.Celebrities == null || !result.Celebrities.Any())
        {
            return NoContent();
        }
        
        return Ok(result);
    }
}