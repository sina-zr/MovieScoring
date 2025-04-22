using Movies.Domain.Entities;

namespace Movies.Domain.Repositories;

public interface ICommentRepository
{
    IQueryable<Comment> GetAllComments();
    Task<Comment?> GetCommentById(ulong id);
    Task<Comment> AddComment(Comment comment);
    Task<IEnumerable<Comment>> AddRangeComments(IEnumerable<Comment> comments);
    Comment UpdateComment(Comment comment);
    IEnumerable<Comment> UpdateRangeComments(IEnumerable<Comment> comments);
    bool DeleteComment(Comment comment);
    bool DeleteRangeComments(IEnumerable<Comment> comments);
}