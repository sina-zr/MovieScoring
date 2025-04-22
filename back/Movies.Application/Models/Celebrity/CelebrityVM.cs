namespace Movies.Application.Models.Celebrity;

public class CelebrityVm
{
    public ulong CelebrityId { get; set; }
    public string FullName { get; set; }
    public int? BirthYear { get; set; }
}

public class CelebrityListVm
{
    public List<CelebrityVm>? Celebrities { get; set; }
    public int PagesCount { get; set; }
}