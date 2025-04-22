using Movies.Application.Models.Movie;
using Movies.Domain.Entities;

namespace Movies.Application.Services.Interfaces;

public interface IMovieService
{
    Task<MovieListVm> GetMovies(int pageId, int pageSize, string? filterTitle, int? genreId);
    Task<Movie?> GetMovieById(ulong movieId);
    Task<MovieDetailsVm?> GetMovieDetails(ulong movieId);
    Task<ulong> AddMovie(AddMovieDto movieDto);
    
    // EditMovie
    // DeleteMovie
}