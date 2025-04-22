using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movies.Domain.Entities;

public class BaseEntity<T>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public T Id { get; set; }
    public DateTime CreateDate { get; set; }
    public bool IsDelete { get; set; }
}