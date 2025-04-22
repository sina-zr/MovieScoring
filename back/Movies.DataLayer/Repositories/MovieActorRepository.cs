using Movies.Domain.Entities;
using Movies.Domain.Repositories;

namespace Movies.DataLayer.Repositories;

public class MovieActorRepository : IMovieActorRepository
{
    private readonly MovieContext _context;

    public MovieActorRepository(MovieContext context)
    {
        _context = context;
    }

    public IQueryable<MovieActor> GetAllMovieActors()
    {
        return _context.MoviesActors.AsQueryable();
    }

    public async Task<MovieActor?> GetMovieActorById(ulong id)
    {
        return await _context.MoviesActors.FindAsync(id);
    }

    public async Task<MovieActor> AddMovieActor(MovieActor movieActor)
    {
        await _context.MoviesActors.AddAsync(movieActor);
        await _context.SaveChangesAsync();
        return movieActor;
    }

    public async Task<IEnumerable<MovieActor>> AddRangeMovieActors(IEnumerable<MovieActor> movieActors)
    {
        await _context.MoviesActors.AddRangeAsync(movieActors);
        await _context.SaveChangesAsync();
        return movieActors;
    }

    public MovieActor UpdateMovieActor(MovieActor movieActor)
    {
        _context.MoviesActors.Update(movieActor);
        _context.SaveChanges();
        return movieActor;
    }

    public IEnumerable<MovieActor> UpdateRangeMovieActors(IEnumerable<MovieActor> movieActors)
    {
        _context.MoviesActors.UpdateRange(movieActors);
        _context.SaveChanges();
        return movieActors;
    }

    public bool DeleteMovieActor(MovieActor movieActor)
    {
        try
        {
            _context.MoviesActors.Remove(movieActor);
            var changes = _context.SaveChanges();
            return changes > 0;
        }
        catch (Exception)
        {
            // Log exception here
            return false;
        }
    }

    public bool DeleteRangeMovieActors(IEnumerable<MovieActor> movieActors)
    {
        try
        {
            _context.MoviesActors.RemoveRange(movieActors);
            var changes = _context.SaveChanges();
            return changes > 0;
        }
        catch (Exception)
        {
            // Log exception here
            return false;
        }
    }
}