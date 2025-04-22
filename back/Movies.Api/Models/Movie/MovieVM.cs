using System.ComponentModel.DataAnnotations;
using Movies.Application.Models.Movie;

namespace Movies.Api.Models.Movie;

public class MoviesParams
{
    public int PageId { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? TitleFilter { get; set; } = string.Empty;
    public int? GenreId { get; set; } = null;
}

public class MoviesListVm
{
    public List<MovieVm>? Movies { get; set; }
    public int PageId { get; set; }
    public int PagesCount { get; set; }
}

public class ScoreMovieDto
{
    public ulong MovieId { get; set; }
    public byte Score { get; set; }
}
