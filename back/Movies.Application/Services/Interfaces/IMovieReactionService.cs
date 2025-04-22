using Movies.Application.Models.Movie;
using Movies.Domain.Entities;

namespace Movies.Application.Services.Interfaces;

public interface IMovieReactionService
{
    Task<bool> AddMovieComment(ulong movieId, ulong userId, string text);
    Task<ScoreMovieResponse> AddMovieScore(ulong movieId, ulong userId, int score);
    Task<UserRate?> GetUserMovieScore(ulong userId, ulong movieId);
    
    // EditComment
    // DeleteComment
}