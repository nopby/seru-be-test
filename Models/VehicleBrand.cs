namespace Seru.BackendTest.Models;

public sealed class VehicleBrand : Entity
{
    public required string Name { get; set; }
    public IEnumerable<VehicleType>? Types { get; set; }
}