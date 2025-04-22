using System.ComponentModel.DataAnnotations.Schema;

namespace Movies.Domain.Entities;

public class Movie : BaseEntity<ulong>
{
    public string Title { get; set; }
    public int Year { get; set; }
    
    #region Relations

    public ICollection<MovieActor>? MovieActors { get; set; }

    public ICollection<MovieDirector>? MovieDirectors { get; set; }

    public ICollection<UserRate>? Rates { get; set; }

    public ICollection<Comment>? Comments { get; set; }

    public ICollection<MovieGenre> Genres { get; set; }

    public ICollection<WatchlistMovie>? WatchlistMovies { get; set; }

    #endregion
}