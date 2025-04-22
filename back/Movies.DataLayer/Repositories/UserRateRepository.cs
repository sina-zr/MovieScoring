using Movies.Domain.Entities;
using Movies.Domain.Repositories;

namespace Movies.DataLayer.Repositories;

public class UserRateRepository : IUserRateRepository
{
    private readonly MovieContext _context;

    public UserRateRepository(MovieContext context)
    {
        _context = context;
    }

    public IQueryable<UserRate> GetAllUserRates()
    {
        return _context.UsersRates.AsQueryable();
    }

    public async Task<UserRate?> GetUserRateById(ulong id)
    {
        return await _context.UsersRates.FindAsync(id);
    }

    public async Task<UserRate> AddUserRate(UserRate userRate)
    {
        await _context.UsersRates.AddAsync(userRate);
        await _context.SaveChangesAsync();
        return userRate;
    }

    public async Task<IEnumerable<UserRate>> AddRangeUserRates(IEnumerable<UserRate> userRates)
    {
        await _context.UsersRates.AddRangeAsync(userRates);
        await _context.SaveChangesAsync();
        return userRates;
    }

    public UserRate UpdateUserRate(UserRate userRate)
    {
        _context.UsersRates.Update(userRate);
        _context.SaveChanges();
        return userRate;
    }

    public IEnumerable<UserRate> UpdateRangeUserRates(IEnumerable<UserRate> userRates)
    {
        _context.UsersRates.UpdateRange(userRates);
        _context.SaveChanges();
        return userRates;
    }

    public bool DeleteUserRate(UserRate userRate)
    {
        try
        {
            _context.UsersRates.Remove(userRate);
            var changes = _context.SaveChanges();
            return changes > 0;
        }
        catch (Exception)
        {
            // Log exception here
            return false;
        }
    }

    public bool DeleteRangeUserRates(IEnumerable<UserRate> userRates)
    {
        try
        {
            _context.UsersRates.RemoveRange(userRates);
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