namespace Movies.Application.Models.Watchlist;

public class WatchlistVm
{
    public List<MovieForWatchlist> Movies { get; set; }
    public string WatchlistName { get; set; }
}

public class MovieForWatchlist
{
    public ulong MovieId { get; set; }
    public string MovieTitle { get; set; }
}