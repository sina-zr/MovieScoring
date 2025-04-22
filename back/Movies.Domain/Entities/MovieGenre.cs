using System.ComponentModel.DataAnnotations.Schema;

namespace Movies.Domain.Entities;

public class MovieGenre:BaseEntityAutoIncrement<ulong>
{
    public int GenreId { get; set; }
    public ulong MovieId { get; set; }

    #region Relations

    [ForeignKey("GenreId")]
    public Genre Genre { get; set; }
    
    [ForeignKey("MovieId")]
    public Movie Movie { get; set; }

    #endregion
}