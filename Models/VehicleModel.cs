namespace Seru.BackendTest.Models;

public sealed class VehicleModel : Entity
{
    public required string Name { get; set; }
    public int TypeId { get; set; }
    public VehicleType? VehicleType { get; set; }
    public ICollection<PriceList> Prices { get; set; }
}