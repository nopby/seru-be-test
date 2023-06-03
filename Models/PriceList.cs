namespace Seru.BackendTest.Models;

public sealed class PriceList : Entity
{
    public required int YearId { get; set; }
    public required VehicleYear VehicleYear { get; set; }
    public required int ModelId { get; set; }
    public required VehicleModel VehicleModel { get; set; }
}