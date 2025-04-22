using Microsoft.EntityFrameworkCore;
using Movies.Application.Models.Celebrity;
using Movies.Application.Models.Movie;
using Movies.Application.Services.Interfaces;
using Movies.Domain.Entities;
using Movies.Domain.Repositories;

namespace Movies.Application.Services.Implementations;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IMovieGenreRepository _movieGenreRepository;

    public MovieService(IMovieRepository movieRepository, IMovieGenreRepository movieGenreRepository)
    {
        _movieRepository = movieRepository;
        _movieGenreRepository = movieGenreRepository;
    }

    public async Task<MovieListVm> GetMovies(int pageId, int pageSize, string? filterTitle, int? genreId)
    {
        var query = _movieRepository.GetAllMovies()
            .Include(m => m.Genres).ThenInclude(g => g.Genre)
            .Include(m => m.Rates)
            .AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(filterTitle))
        {
            query = query.Where(m => m.Title.Contains(filterTitle));
        }

        if (genreId.HasValue)
        {
            query = query.Where(m => m.Genres.Any(g => g.GenreId == genreId.Value));
        }

        #region Pagination

        long totalCount = await query.LongCountAsync();
        int pageCount = (int)Math.Ceiling((double)totalCount / pageSize);

        if (totalCount == 0)
        {
            return new MovieListVm
            {
                Movies = null,
                PagesCount = 0
            };
        }

        pageId = Math.Clamp(pageId, 1, pageCount);
        int skip = (pageId - 1) * pageSize;

        #endregion

        var movies = await query.OrderByDescending(m => m.Rates.Average(r => r.Score)).Skip(skip).Take(pageSize)
            .ToListAsync();

        return new MovieListVm
        {
            Movies = movies.Select(m => new MovieVm()
            {
                MovieId = m.Id,
                Title = m.Title,
                Year = m.Year,
                Genres = m.Genres.Select(g => g.Genre.Title).ToList(),
                Score = (m.Rates.Any()) ? m.Rates.Average(r => r.Score) : 0,
            }).ToList(),
            PagesCount = pageCount
        };
    }

    public async Task<Movie?> GetMovieById(ulong movieId)
    {
        return await _movieRepository.GetMovieById(movieId);
    }

    public async Task<MovieDetailsVm?> GetMovieDetails(ulong movieId)
    {
        var movie = await _movieRepository.GetAllMovies()
            .Include(m => m.Genres).ThenInclude(g => g.Genre)
            .Include(m => m.Rates)
            .Include(m => m.MovieActors).ThenInclude(ma => ma.CinemaPeople)
            .Include(m => m.MovieDirectors).ThenInclude(ma => ma.CinemaPeople)
            .Include(m => m.Comments).ThenInclude(c => c.User)
            .FirstOrDefaultAsync(m => m.Id == movieId);

        if (movie == null)
            return null;

        var output = new MovieDetailsVm()
        {
            MovieId = movie.Id,
            Title = movie.Title,
            Year = movie.Year,
            Genres = movie.Genres.Select(g => g.Genre.Title).ToList(),
            Actors = movie.MovieActors?.Select(a => new CelebrityVm()
            {
                CelebrityId = a.CinemaPeople.Id,
                FullName = a.CinemaPeople.FullName,
                BirthYear = a.CinemaPeople.BirthYear,
            }).ToList(),
            Directors = movie.MovieDirectors?.Select(d => new CelebrityVm()
            {
                CelebrityId = d.CinemaPeople.Id,
                FullName = d.CinemaPeople.FullName,
                BirthYear = d.CinemaPeople.BirthYear,
            }).ToList(),
            Comments = movie.Comments?.Select(c => new CommentVm()
            {
                CommentId = c.Id,
                Text = c.Text,
                CommenterId = c.User.Id,
                UserFullName = c.User.FullName
            }).ToList(),
            Score = (movie.Rates != null && movie.Rates.Any())
                ? movie.Rates.Average(r => r.Score)
                : 0,
        };

        return output;
    }
    
    public async Task<ulong> AddMovie(AddMovieDto addMovieDto)
    {
        if (string.IsNullOrEmpty(addMovieDto.Title) || addMovieDto.Year < 1800 ||
            addMovieDto.Year > DateTime.UtcNow.Year)
        {
            return 0;
        }

        var lastId = await _movieRepository.FindLastId();
        if (lastId < 1)
        {
            return 0;
        }

        var newMovie = await _movieRepository.AddMovie(new Movie()
        {
            Title = addMovieDto.Title,
            Year = addMovieDto.Year,
            Id = lastId + 1
        });

        if (addMovieDto.Genres?.Count > 0)
        {
            await _movieGenreRepository.AddRangeMovieGenres(addMovieDto.Genres.Select(gId => new MovieGenre()
            {
                MovieId = newMovie.Id,
                GenreId = gId, CreateDate = DateTime.UtcNow, IsDelete = false
            }).ToList());
        }

        return newMovie.Id;
    }
}