using Microsoft.EntityFrameworkCore;
using Movies.Application.Models.Celebrity;
using Movies.Application.Services.Interfaces;
using Movies.Domain.Entities;
using Movies.Domain.Repositories;

namespace Movies.Application.Services.Implementations;

public class CelebrityManagementService : ICelebrityManagementService
{
    private readonly ICinemaPeopleRepository _cinemaPeopleRepository;
    private readonly IMovieActorRepository _movieActorRepository;
    private readonly IMovieDirectorRepository _movieDirectorRepository;

    public CelebrityManagementService(ICinemaPeopleRepository cinemaPeopleRepository, IMovieActorRepository movieActorRepository,
        IMovieDirectorRepository movieDirectorRepository)
    {
        _cinemaPeopleRepository = cinemaPeopleRepository;
        _movieActorRepository = movieActorRepository;
        _movieDirectorRepository = movieDirectorRepository;
    }

    public async Task<CelebrityListVm> GetAllCelebrities(int pageId, int pageSize, string? filterName)
    {
        var query = _cinemaPeopleRepository.GetAllCinemaPeople()
            .AsQueryable();

        if (!string.IsNullOrEmpty(filterName))
        {
            query = query.Where(q => q.FullName.Contains(filterName));
        }
        
        #region Pagination

        long totalCount = await query.LongCountAsync();
        int pageCount = (int)Math.Ceiling((double)totalCount / pageSize);

        if (totalCount == 0)
        {
            return new CelebrityListVm()
            {
                Celebrities = null,
                PagesCount = 0
            };
        }

        pageId = Math.Clamp(pageId, 1, pageCount);
        int skip = (pageId - 1) * pageSize;

        #endregion

        return new CelebrityListVm()
        {
            Celebrities = await query.Skip(skip).Take(pageSize)
                .Select(c => new CelebrityVm()
                {
                    FullName = c.FullName,
                    BirthYear = c.BirthYear,
                    CelebrityId = c.Id
                }).ToListAsync(),
            PagesCount = pageCount,
        };
    }
    public async Task<bool> AddCinemaPeople(string fullName, int birthYear)
    {
        var cinemaPeoples = new CinemaPeople()
        {
            FullName = fullName,
            BirthYear = birthYear,
            CreateDate = DateTime.UtcNow,
            IsDelete = false
        };
        
        var lastId= await _cinemaPeopleRepository.GetLastCinemaPeopleId();
        cinemaPeoples.Id = lastId + 1;

        try
        {
            await _cinemaPeopleRepository.AddCinemaPeople(cinemaPeoples);
            return true;
        }
        catch (Exception e)
        {
            // log
            return false;
        }
    }

    public async Task<bool> UpsertMovieActors(ulong movieId, List<ulong> actorIds)
    {
        if (!actorIds.Any())
        {
            return false;
        }
        
        // Get existing movie actors
        var existingMovieActors = await _movieActorRepository.GetAllMovieActors()
            .Where(ma => actorIds.Contains(ma.CinemaPeopleId) && ma.MovieId == movieId).ToListAsync();
        
        // Find actors to keep
        var actorsToKeep = existingMovieActors
            .Where(ma => actorIds.Contains(ma.CinemaPeopleId))
            .ToList();
        
        // Find actors to remove
        var actorsToRemove = existingMovieActors
            .Where(ma => !actorIds.Contains(ma.CinemaPeopleId))
            .ToList();
        
        // Find actors to add
        var existingActorIds = existingMovieActors.Select(ma => ma.CinemaPeopleId).ToList();
        var actorsToAdd = actorIds
            .Where(id => !existingActorIds.Contains(id))
            .Select(actorId => new MovieActor { MovieId = movieId, CinemaPeopleId = actorId })
            .ToList();

        // Remove actors that are no longer needed
        actorsToRemove.ForEach(a =>
            a.IsDelete = true);
        _movieActorRepository.UpdateRangeMovieActors(actorsToRemove);

        // Add new actors
        await _movieActorRepository.AddRangeMovieActors(actorsToAdd);

        return true;
    }

    public async Task<bool> UpsertMovieDirectors(ulong movieId, List<ulong> directorIds)
    {
        if (!directorIds.Any())
        {
            return false;
        }

        // Get existing movie directors
        var existingMovieDirectors = await _movieDirectorRepository.GetAllMovieDirectors()
            .Where(md => directorIds.Contains(md.CinemaPeopleId) && md.MovieId == movieId)
            .ToListAsync();

        // Find directors to keep
        var directorsToKeep = existingMovieDirectors
            .Where(md => directorIds.Contains(md.CinemaPeopleId))
            .ToList();

        // Find directors to remove
        var directorsToRemove = existingMovieDirectors
            .Where(md => !directorIds.Contains(md.CinemaPeopleId))
            .ToList();

        // Find directors to add
        var existingDirectorIds = existingMovieDirectors.Select(md => md.CinemaPeopleId).ToList();
        var directorsToAdd = directorIds
            .Where(id => !existingDirectorIds.Contains(id))
            .Select(directorId => new MovieDirector { MovieId = movieId, CinemaPeopleId = directorId })
            .ToList();

        // Remove directors that are no longer needed
        directorsToRemove.ForEach(d =>
            d.IsDelete = true);
        _movieDirectorRepository.UpdateRangeMovieDirectors(directorsToRemove);

        // Add new directors
        await _movieDirectorRepository.AddRangeMovieDirectors(directorsToAdd);

        return true;
    }
}