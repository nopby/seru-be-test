using System.ComponentModel.DataAnnotations;

namespace Seru.BackendTest.Models;


public class Entity
{
    [Key]
    public int Id { get; init; }
    [Required]
    public DateTime Created_At { get; init; } = DateTime.UtcNow;
    [Required]
    public DateTime Updated_At { get; set;} = DateTime.UtcNow;
}