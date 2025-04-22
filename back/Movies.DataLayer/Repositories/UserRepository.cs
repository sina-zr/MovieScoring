using Movies.Domain.Entities;
using Movies.Domain.Repositories;

namespace Movies.DataLayer.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MovieContext _context;

    public UserRepository(MovieContext context)
    {
        _context = context;
    }

    public IQueryable<User> GetAllUsers()
    {
        return _context.Users.AsQueryable();
    }

    public async Task<User?> GetUserById(ulong id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User> AddUser(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<IEnumerable<User>> AddRangeUsers(IEnumerable<User> users)
    {
        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();
        return users;
    }

    public User UpdateUser(User user)
    {
        _context.Users.Update(user);
        _context.SaveChanges();
        return user;
    }

    public IEnumerable<User> UpdateRangeUsers(IEnumerable<User> users)
    {
        _context.Users.UpdateRange(users);
        _context.SaveChanges();
        return users;
    }

    public bool DeleteUser(User user)
    {
        try
        {
            _context.Users.Remove(user);
            var changes = _context.SaveChanges();
            return changes > 0;
        }
        catch (Exception)
        {
            // Log exception here
            return false;
        }
    }

    public bool DeleteRangeUsers(IEnumerable<User> users)
    {
        try
        {
            _context.Users.RemoveRange(users);
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