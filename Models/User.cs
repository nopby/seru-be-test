namespace Seru.BackendTest.Models;

public sealed class User : Entity
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required bool IsAdmin { get; set; }
}

