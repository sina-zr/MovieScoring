using Movies.Domain.Enums;

namespace Movies.Domain.Entities;

public class User:BaseEntityAutoIncrement<ulong>
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
    public UserRoles Role { get; set; }

    #region Relations

    public ICollection<UserRate>? UserRates { get; set; }

    public ICollection<Comment>? UserComments { get; set; }

    public ICollection<Watchlist>? Watchlists { get; set; }

    #endregion
}