using Microsoft.EntityFrameworkCore;
using Movies.Domain.Entities;
using Movies.Domain.Repositories;

namespace Movies.DataLayer.Repositories;

public class CinemaPeopleRepository : ICinemaPeopleRepository
{
    private readonly MovieContext _context;

    public CinemaPeopleRepository(MovieContext context)
    {
        _context = context;
    }

    public IQueryable<CinemaPeople> GetAllCinemaPeople()
    {
        return _context.CinemaPeople.AsQueryable();
    }

    public async Task<CinemaPeople?> GetCinemaPeopleById(ulong id)
    {
        return await _context.CinemaPeople.FindAsync(id);
    }

    public async Task<CinemaPeople> AddCinemaPeople(CinemaPeople cinemaPeople)
    {
        await _context.CinemaPeople.AddAsync(cinemaPeople);
        await _context.SaveChangesAsync();
        return cinemaPeople;
    }

    public async Task<IEnumerable<CinemaPeople>> AddRangeCinemaPeople(IEnumerable<CinemaPeople> cinemaPeople)
    {
        await _context.CinemaPeople.AddRangeAsync(cinemaPeople);
        await _context.SaveChangesAsync();
        return cinemaPeople;
    }

    public CinemaPeople UpdateCinemaPeople(CinemaPeople cinemaPeople)
    {
        _context.CinemaPeople.Update(cinemaPeople);
        _context.SaveChanges();
        return cinemaPeople;
    }

    public IEnumerable<CinemaPeople> UpdateRangeCinemaPeople(IEnumerable<CinemaPeople> cinemaPeople)
    {
        _context.CinemaPeople.UpdateRange(cinemaPeople);
        _context.SaveChanges();
        return cinemaPeople;
    }

    public bool DeleteCinemaPeople(CinemaPeople cinemaPeople)
    {
        try
        {
            _context.CinemaPeople.Remove(cinemaPeople);
            var changes = _context.SaveChanges();
            return changes > 0;
        }
        catch (Exception)
        {
            // Log exception here
            return false;
        }
    }

    public bool DeleteRangeCinemaPeople(IEnumerable<CinemaPeople> cinemaPeople)
    {
        try
        {
            _context.CinemaPeople.RemoveRange(cinemaPeople);
            var changes = _context.SaveChanges();
            return changes > 0;
        }
        catch (Exception)
        {
            // Log exception here
            return false;
        }
    }

    public async Task<ulong> GetLastCinemaPeopleId()
    {
        return await _context.CinemaPeople.MaxAsync(c => c.Id);
    }
}