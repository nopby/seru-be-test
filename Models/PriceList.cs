using System.Text.Json.Serialization;

namespace Seru.BackendTest.Models;

public sealed class PriceList : Entity
{
    public int YearId { get; set; }
    public required VehicleYear VehicleYear { get; set; }
    public int ModelId { get; set; }
    public required VehicleModel VehicleModel { get; set; }
}