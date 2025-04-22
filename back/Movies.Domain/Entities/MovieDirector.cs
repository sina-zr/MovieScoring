using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movies.Domain.Entities;

public class MovieDirector : BaseEntity<ulong>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public new ulong Id { get; set; }
    
    public ulong MovieId { get; set; }
    public ulong CinemaPeopleId { get; set; }
    
    #region Relations

    [ForeignKey("CinemaPeopleId")]
    public CinemaPeople CinemaPeople { get; set; }

    [ForeignKey("MovieId")]
    public Movie Movie { get; set; }

    #endregion
}