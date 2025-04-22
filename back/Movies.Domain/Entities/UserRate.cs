using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movies.Domain.Entities;

public class UserRate:BaseEntityAutoIncrement<ulong>
{
    public ulong MovieId { get; set; }
    public ulong UserId { get; set; }
    [Range(1, 10,ErrorMessage = "Score must be between 1 and 10.")]
    public byte Score { get; set; }

    #region Relations
    [ForeignKey("MovieId")]
    public Movie Movie { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }
    
    #endregion
}