namespace Seru.BackendTest.Models;

public sealed class VehicleYear : Entity
{
    public required string Year { get; set; }
    public ICollection<PriceList> Prices { get; set; }
}