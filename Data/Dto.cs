namespace Seru.BackendTest.Data;


public sealed record LoginDto(string Email, string Password);
public sealed record VehicleBrandDto(string Name);
public sealed record UserDto(
    string Email,
    string Password,
    string Name,
    bool IsAdmin);
public sealed record VehicleTypeDto(string Name, int Brand_id);
public sealed record VehicleModelDto(string Name, int Type_id);
public sealed record VehicleYearDto(string Year);
public sealed record PriceListDto(string Year, string Model);