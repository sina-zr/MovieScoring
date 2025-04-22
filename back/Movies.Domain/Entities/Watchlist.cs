using System.ComponentModel.DataAnnotations.Schema;

namespace Movies.Domain.Entities;

public class Watchlist: BaseEntityAutoIncrement<ulong>
{
    public string WatchListName { get; set; }
    public ulong UserId { get; set; }

    #region Relations

    [ForeignKey("UserId")]
    public User User { get; set; }

    public ICollection<WatchlistMovie> WatchlistMovies { get; set; }

    #endregion
}