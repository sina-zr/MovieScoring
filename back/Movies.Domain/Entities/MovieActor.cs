using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movies.Domain.Entities;

public class MovieActor : BaseEntityAutoIncrement<ulong>
{
    public ulong CinemaPeopleId { get; set; }
    public ulong MovieId { get; set; }

    #region Relations

    [ForeignKey("CinemaPeopleId")]
    public CinemaPeople CinemaPeople { get; set; }

    [ForeignKey("MovieId")]
    public Movie Movie { get; set; }

    #endregion
}