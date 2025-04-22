namespace Movies.Domain.Entities;

public class CinemaPeople : BaseEntity<ulong>
{
    public string FullName { get; set; }
    public int? BirthYear { get; set; }

    #region Relations

    public ICollection<MovieDirector>? DirectorMovies { get; set; }
    public ICollection<MovieActor>? ActorMovies { get; set; }

    #endregion
}