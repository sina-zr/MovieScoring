using Microsoft.EntityFrameworkCore;
using Movies.Application.Models.Movie;
using Movies.Application.Services.Interfaces;
using Movies.Domain.Entities;
using Movies.Domain.Repositories;

namespace Movies.Application.Services.Implementations;

public class MovieReactionService : IMovieReactionService
{
    private readonly IUserRateRepository _userRateRepository;
    private readonly ICommentRepository _commentRepository;

    public MovieReactionService(IUserRateRepository userRateRepository, ICommentRepository commentRepository)
    {
        _userRateRepository = userRateRepository;
        _commentRepository = commentRepository;
    }
    
    public async Task<ScoreMovieResponse> AddMovieScore(ulong movieId, ulong userId, int score)
    {
        // Validate score range (assuming 1-10 scale)
        if (score < 1 || score > 10)
        {
            return ScoreMovieResponse.InvalidScore;
        }

        // Check if user has already scored this movie
        var existingScore = _userRateRepository.GetAllUserRates().FirstOrDefault(r => r.UserId == userId
            && r.MovieId == movieId);

        if (existingScore != null)
        {
            return ScoreMovieResponse.AlreadyScored;
        }

        // Add the new score
        var userRate = new UserRate
        {
            MovieId = movieId,
            UserId = userId,
            Score = (byte)score,
            CreateDate = DateTime.UtcNow,
            IsDelete = false
        };

        try
        {
            await _userRateRepository.AddUserRate(userRate);
            return ScoreMovieResponse.Success;
        }
        catch (Exception e)
        {
            //log
            return ScoreMovieResponse.UnknownError;
        }
    }

    public async Task<UserRate?> GetUserMovieScore(ulong userId, ulong movieId)
    {
        var score = await _userRateRepository.GetAllUserRates()
            .FirstOrDefaultAsync(r => r.UserId == userId && r.MovieId == movieId);

        return score;
    }
    public async Task<bool> AddMovieComment(ulong movieId, ulong userId, string text)
    {
        if (userId < 1 || movieId < 1)
        {
            return false;
        }

        if (string.IsNullOrEmpty(text))
        {
            return false;
        }
        
        try
        {
            await _commentRepository.AddComment(new Comment()
            {
                MovieId = movieId,
                UserId = userId,
                Text = text,
                CreateDate = DateTime.UtcNow,
                IsDelete = false
            });
            return true;
        }
        catch (Exception e)
        {
            // log
            return false;
        }
    }
}