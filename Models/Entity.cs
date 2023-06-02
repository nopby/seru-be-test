namespace Seru.BackendTest.Models;


public class Entity
{
    public required int Id { get; init; }
    public required DateTime Created_At { get; init; } = DateTime.UtcNow;
    public required DateTime Updated_At { get; set;} = DateTime.UtcNow;
}