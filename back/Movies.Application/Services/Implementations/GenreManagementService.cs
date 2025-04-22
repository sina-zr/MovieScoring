using Microsoft.EntityFrameworkCore;
using Movies.Application.Models.Genre;
using Movies.Application.Services.Interfaces;
using Movies.Domain.Entities;
using Movies.Domain.Repositories;

namespace Movies.Application.Services.Implementations;

public class GenreManagementService : IGenreManagementService
{
    private readonly IGenreRepository _genreRepository;
    private readonly IMovieGenreRepository _movieGenreRepository;

    public GenreManagementService(IGenreRepository genreRepository, IMovieGenreRepository movieGenreRepository)
    {
        _genreRepository = genreRepository;
        _movieGenreRepository = movieGenreRepository;
    }
    
    public async Task<List<GenreVm>> GetAllGenres()
    {
        return await _genreRepository.GetAllGenres()
            .Select(g=>new GenreVm
            {
                GenreId = g.Id,
                Title = g.Title
            })
            .AsNoTracking().ToListAsync();
    }

    public async Task<bool> UpsertMovieGenres(ulong movieId, List<int> genreIds)
    {
        if (!genreIds.Any())
        {
            return false;
        }

        // Get existing movie genres
        var existingMovieGenres = await _movieGenreRepository.GetAllMovieGenres()
            .Where(mg => genreIds.Contains(mg.GenreId) && mg.MovieId == movieId)
            .ToListAsync();

        // Find genres to keep
        var genresToKeep = existingMovieGenres
            .Where(mg => genreIds.Contains(mg.GenreId))
            .ToList();

        // Find genres to remove
        var genresToRemove = existingMovieGenres
            .Where(mg => !genreIds.Contains(mg.GenreId))
            .ToList();

        // Find genres to add
        var existingGenreIds = existingMovieGenres.Select(mg => mg.GenreId).ToList();
        var genresToAdd = genreIds
            .Where(id => !existingGenreIds.Contains(id))
            .Select(genreId => new MovieGenre { MovieId = movieId, GenreId = genreId })
            .ToList();

        // Remove genres that are no longer needed
        genresToRemove.ForEach(g =>
            g.IsDelete = true);
        _movieGenreRepository.UpdateRangeMovieGenres(genresToRemove);

        // Add new genres
        await _movieGenreRepository.AddRangeMovieGenres(genresToAdd);

        return true;
    }
}