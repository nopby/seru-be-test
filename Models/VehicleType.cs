namespace Seru.BackendTest.Models;

public sealed class VehicleType : Entity
{
    public required string Name { get; set; }
    public int BrandId { get; set; }
    public VehicleBrand? VehicleBrand { get; set; }
    public IEnumerable<VehicleModel>? VehicleModels { get; set; }
}