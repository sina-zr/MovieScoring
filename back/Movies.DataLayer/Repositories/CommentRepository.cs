using Movies.Domain.Entities;
using Movies.Domain.Repositories;

namespace Movies.DataLayer.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly MovieContext _context;

    public CommentRepository(MovieContext context)
    {
        _context = context;
    }
    
    public IQueryable<Comment> GetAllComments()
    {
        return _context.Comments.AsQueryable();
    }

    public async Task<Comment?> GetCommentById(ulong id)
    {
        return await _context.Comments.FindAsync(id);
    }

    public async Task<Comment> AddComment(Comment comment)
    {
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();
        return comment;
    }

    public async Task<IEnumerable<Comment>> AddRangeComments(IEnumerable<Comment> comments)
    {
        await _context.Comments.AddRangeAsync(comments);
        await _context.SaveChangesAsync();
        return comments;
    }

    public Comment UpdateComment(Comment comment)
    {
        _context.Comments.Update(comment);
        _context.SaveChanges();
        return comment;
    }

    public IEnumerable<Comment> UpdateRangeComments(IEnumerable<Comment> comments)
    {
        _context.Comments.UpdateRange(comments);
        _context.SaveChanges();
        return comments;
    }

    public bool DeleteComment(Comment comment)
    {
        try
        {
            _context.Comments.Remove(comment);
            var changes = _context.SaveChanges();
            return changes > 0;
        }
        catch (Exception)
        {
            // Log exception here
            return false;
        }
    }

    public bool DeleteRangeComments(IEnumerable<Comment> comments)
    {
        try
        {
            _context.Comments.RemoveRange(comments);
            var changes = _context.SaveChanges();
            return changes > 0;
        }
        catch (Exception)
        {
            // Log exception here
            return false;
        }
    }
}