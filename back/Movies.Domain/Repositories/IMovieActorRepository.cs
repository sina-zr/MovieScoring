using Movies.Domain.Entities;

namespace Movies.Domain.Repositories;

public interface IMovieActorRepository
{
    IQueryable<MovieActor> GetAllMovieActors();
    Task<MovieActor?> GetMovieActorById(ulong id);
    Task<MovieActor> AddMovieActor(MovieActor movieActor);
    Task<IEnumerable<MovieActor>> AddRangeMovieActors(IEnumerable<MovieActor> movieActors);
    MovieActor UpdateMovieActor(MovieActor movieActor);
    IEnumerable<MovieActor> UpdateRangeMovieActors(IEnumerable<MovieActor> movieActors);
    bool DeleteMovieActor(MovieActor movieActor);
    bool DeleteRangeMovieActors(IEnumerable<MovieActor> movieActors);
}