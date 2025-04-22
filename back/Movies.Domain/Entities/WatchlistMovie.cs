using System.ComponentModel.DataAnnotations.Schema;

namespace Movies.Domain.Entities;

public class WatchlistMovie:BaseEntityAutoIncrement<ulong>
{
    public ulong WatchlistId { get; set; }
    public ulong MovieId { get; set; }

    #region Relations
    
    [ForeignKey("WatchlistId")]
    public Watchlist Watchlist { get; set; }

    [ForeignKey("MovieId")]
    public Movie Movie { get; set; }

    #endregion
}