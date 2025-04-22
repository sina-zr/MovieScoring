using Movies.Application.Models.Genre;

namespace Movies.Application.Services.Interfaces;

public interface IGenreManagementService
{
    Task<List<GenreVm>> GetAllGenres();
    
    Task<bool> UpsertMovieGenres(ulong movieId, List<int> genreIds);
    
    // DeleteAllMovieGenres
}