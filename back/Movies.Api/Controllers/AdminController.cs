using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Models.Admin;
using Movies.Application.Models.Celebrity;
using Movies.Application.Models.Movie;
using Movies.Application.Models.User;
using Movies.Application.Services.Interfaces;

namespace Movies.Api.Controllers;

[ApiController]
[Authorize("AdminOnly")]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly ICelebrityManagementService _celebrityManagementService;
    private readonly IMovieService _movieService;
    private readonly IUserManagementService _userManagementService;
    private readonly IGenreManagementService _genreManagementService;

    public AdminController(ICelebrityManagementService celebrityManagementService, IMovieService movieService,
        IUserManagementService userManagementService, IGenreManagementService genreManagementService)
    {
        _celebrityManagementService = celebrityManagementService;
        _movieService = movieService;
        _userManagementService = userManagementService;
        _genreManagementService = genreManagementService;
    }

    #region User Management

    /// <summary>
    /// Add User from Admin
    /// </summary>
    /// <param name="addUserDto"></param>
    /// <returns>Http Status Code</returns>
    [HttpPost("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddUser([FromBody] AddUserDto addUserDto)
    {
        var response = await _userManagementService.AddUser(addUserDto);
        switch (response)
        {
            case AddUserResponse.Success:
                return Ok();
            case AddUserResponse.EmptyUsernameOrPassword:
                return BadRequest("Username or password is incorrect");
            case AddUserResponse.UsernameExists:
                return BadRequest("Username already exists");
            case AddUserResponse.UnKnownError:
                return StatusCode(500);
            default:
                return StatusCode(500);
        }
    }

    #endregion

    #region Movie Management

    /// <summary>
    /// Add Celebrity, (Admin only)
    /// Celebrities are used for determining Movie Actors and Directors (with different endpoints)
    /// </summary>
    /// <param name="addDto"></param>
    /// <returns>Http status codes</returns>
    [HttpPost("AddCelebrity")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddCinemaPeople(AddCinemaPeopleDto addDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        if (await _celebrityManagementService.AddCinemaPeople(addDto.FullName, addDto.BirthYear))
        {
            return Ok();
        }

        return StatusCode(500);
    }

    /// <summary>
    /// Add Movie, (Aadmin only)
    /// </summary>
    /// <param name="addDto"></param>
    /// <returns></returns>
    [HttpPost("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddMovie([FromBody] AddMovieDto addDto)
    {
        if (string.IsNullOrEmpty(addDto.Title))
        {
            return BadRequest();
        }

        var movieId = await _movieService.AddMovie(addDto);
        if (movieId > 0)
        {
            return Ok(movieId);
        }

        return StatusCode(500);
    }

    /// <summary>
    /// Add Actors to a movie, or update them (Admin only)
    /// </summary>
    /// <param name="movieId"></param>
    /// <param name="actorIds">list/array of actor IDs, taken from GetAllCelebrities endpoint</param>
    /// <returns>Http status codes</returns>
    [HttpPost("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpsertMovieActors([Required] ulong movieId, [FromBody] List<ulong> actorIds)
    {
        var movie = await _movieService.GetMovieById(movieId);
        if (movie == null)
        {
            return NotFound("Movie not found");
        }

        var isSuccess = await _celebrityManagementService.UpsertMovieActors(movieId, actorIds);
        if (isSuccess)
        {
            return Ok();
        }

        return StatusCode(500);
    }

    /// <summary>
    /// Add Directors to a movie, or update them (Admin only)
    /// </summary>
    /// <param name="movieId"></param>
    /// <param name="directorIds">list/array of director IDs, taken from GetAllCelebrities endpoint</param>
    /// <returns>Http status codes</returns>
    [HttpPost("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpsertMovieDirectors([Required] ulong movieId, [FromBody] List<ulong> directorIds)
    {
        var movie = await _movieService.GetMovieById(movieId);
        if (movie == null)
        {
            return NotFound("Movie not found");
        }

        var isSuccess = await _celebrityManagementService.UpsertMovieDirectors(movieId, directorIds);
        if (isSuccess)
        {
            return Ok();
        }

        return StatusCode(500);
    }

    /// <summary>
    /// Add genres to a movie, or update them (Admin only)
    /// </summary>
    /// <param name="movieId"></param>
    /// <param name="genreIds">list/array of director IDs, taken from GetAllGenres endpoint</param>
    /// <returns>Http status codes</returns>
    [HttpPost("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpsertMovieGenres([Required] ulong movieId, [FromBody] List<int> genreIds)
    {
        var movie = await _movieService.GetMovieById(movieId);
        if (movie == null)
        {
            return NotFound("Movie not found");
        }

        var isSuccess = await _genreManagementService.UpsertMovieGenres(movieId, genreIds);
        if (isSuccess)
        {
            return Ok();
        }

        return StatusCode(500);
    }

    #endregion
}