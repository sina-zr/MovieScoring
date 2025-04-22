using Microsoft.EntityFrameworkCore;
using Movies.Domain.Entities;

namespace Movies.DataLayer;

public class MovieContext : DbContext
{
    public MovieContext(DbContextOptions<MovieContext> options): base(options)
    {
        
    }
    
    public DbSet<Movie> Movies { get; set; }
    public DbSet<CinemaPeople> CinemaPeople { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<MovieActor> MoviesActors { get; set; }
    public DbSet<MovieDirector> MoviesDirectors { get; set; }
    public DbSet<UserRate> UsersRates { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<MovieGenre> MoviesGenres { get; set; }
    public DbSet<Watchlist> Watchlists { get; set; }
    public DbSet<WatchlistMovie> WatchlistsMovies { get; set; }
}