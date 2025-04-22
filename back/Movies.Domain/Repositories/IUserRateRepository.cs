using Movies.Domain.Entities;

namespace Movies.Domain.Repositories;

public interface IUserRateRepository
{
    IQueryable<UserRate> GetAllUserRates();
    Task<UserRate?> GetUserRateById(ulong id);
    Task<UserRate> AddUserRate(UserRate userRate);
    Task<IEnumerable<UserRate>> AddRangeUserRates(IEnumerable<UserRate> userRates);
    UserRate UpdateUserRate(UserRate userRate);
    IEnumerable<UserRate> UpdateRangeUserRates(IEnumerable<UserRate> userRates);
    bool DeleteUserRate(UserRate userRate);
    bool DeleteRangeUserRates(IEnumerable<UserRate> userRates);
}