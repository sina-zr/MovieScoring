using Movies.Domain.Entities;

namespace Movies.Domain.Repositories;

public interface IUserRepository
{
    IQueryable<User> GetAllUsers();
    Task<User?> GetUserById(ulong id);
    Task<User> AddUser(User user);
    Task<IEnumerable<User>> AddRangeUsers(IEnumerable<User> users);
    User UpdateUser(User user);
    IEnumerable<User> UpdateRangeUsers(IEnumerable<User> users);
    bool DeleteUser(User user);
    bool DeleteRangeUsers(IEnumerable<User> users);
}