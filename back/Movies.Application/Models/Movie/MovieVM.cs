namespace Movies.Application.Models.Movie;

public class MovieVm
{
    public ulong MovieId { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }
    public List<string>? Genres { get; set; }
    public double Score { get; set; }
}

public class MovieListVm
{
    public List<MovieVm>? Movies { get; set; }
    public int PagesCount { get; set; }
}

public class AddMovieDto
{
    public string Title { get; set; }
    public int Year { get; set; }
    public List<int>? Genres { get; set; }
}

public class MovieDetailsVm
{
    public ulong MovieId { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }
    public List<string>? Genres { get; set; }
    public double Score { get; set; } = 0;
    public List<Celebrity.CelebrityVm>? Actors { get; set; }
    public List<Celebrity.CelebrityVm>? Directors { get; set; }
    public List<CommentVm>? Comments { get; set; }
}

public class CommentVm
{
    public ulong CommentId { get; set; }
    public string Text { get; set; }
    public ulong CommenterId { get; set; }
    public string UserFullName { get; set; }
}