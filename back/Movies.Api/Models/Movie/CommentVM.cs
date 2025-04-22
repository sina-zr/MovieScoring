using System.ComponentModel.DataAnnotations;

namespace Movies.Api.Models.Movie;

public class AddCommentDto
{
    public ulong MovieId { get; set; }
    [Required] public string Text { get; set; }
}