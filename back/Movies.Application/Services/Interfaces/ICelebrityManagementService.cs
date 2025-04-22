using Movies.Application.Models.Celebrity;

namespace Movies.Application.Services.Interfaces;

public interface ICelebrityManagementService
{
    // GetAllCelebrities
    Task<CelebrityListVm> GetAllCelebrities(int pageId, int pageSize, string? filterName);
    Task<bool> AddCinemaPeople(string fullName, int birthYear);
    
    // Edit Celebrity
    // Delete Celebrity
    
    Task<bool> UpsertMovieActors(ulong movieId, List<ulong> actorIds);
    
    Task<bool> UpsertMovieDirectors(ulong movieId, List<ulong> directorIds);
}