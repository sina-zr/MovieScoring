using System.ComponentModel.DataAnnotations;

namespace Movies.Api.Models.Admin;

public class AddCinemaPeopleDto
{
    [Required]
    public string FullName { get; set; }
    [Required]
    public int BirthYear { get; set; }
}