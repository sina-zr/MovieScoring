namespace Movies.Domain.Entities;

public class Genre:BaseEntityAutoIncrement<int>
{
    public string Title { get; set; }

    #region Relations

    public ICollection<MovieGenre> Movies { get; set; }

    #endregion
}