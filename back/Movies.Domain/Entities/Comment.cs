using System.ComponentModel.DataAnnotations.Schema;

namespace Movies.Domain.Entities;

public class Comment : BaseEntity<ulong>
{
    public ulong MovieId { get; set; }
    public ulong UserId { get; set; }
    public string Text { get; set; }

    #region Relations

    [ForeignKey("MovieId")]
    public Movie Movie { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }
    
    #endregion
}