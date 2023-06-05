using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seru.BackendTest.Data;
using Seru.BackendTest.Models;

namespace Seru.BackendTest.Controllers;

[ApiController]
[Route("vehicle-types")]
[Authorize]
public sealed class VehicleTypesController : ControllerBase
{
    private readonly AppDbContext _context;

    public VehicleTypesController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<IActionResult> GetAllVehicleTypes([FromQuery] int? brand_id)
    {
        IQueryable<VehicleType> query = _context.VehicleTypes;
        if (brand_id.HasValue)
        {
            query = query.Where(x => x.BrandId == brand_id);
        }
        var vehicleTypes = await query.Select(x => new
        {
            Id = x.Id,
            Name = x.Name,
            Brand_id = x.BrandId,
            Created_at = x.Created_At,
            Update_at = x.Updated_At
        }).ToListAsync();

        return Ok(vehicleTypes);
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetVehicleType(int id)
    {
        var vehicleType = await _context.VehicleTypes.FindAsync(id);
        if (vehicleType == null)
        {
            return NotFound("Vehicle type not found");
        }
        return Ok(new
        {
            Id = vehicleType.Id,
            Name = vehicleType.Name,
            Brand_id = vehicleType.BrandId,
            Create_at = vehicleType.Created_At,
            Update_at = vehicleType.Updated_At
        });
    }

    [HttpGet("page/{page:int}")]
    public async Task<IActionResult> GetVehicleTypes(int page = 1)
    {
        if (page <= 0)
        {
            return BadRequest("Invalid Parameter");
        }
        if (_context.VehicleTypes == null)
            return NotFound("Types not found");

        var pageResult = 10;
        var pageCount = Math.Ceiling(_context.VehicleTypes.Count() / (float)pageResult);
        var skip = (page - 1) * (int)pageResult;
        var vehicleTypes = await _context.VehicleTypes
            .Select(x => new
            {
                Id = x.Id,
                Name = x.Name,
                Brand_id = x.BrandId,
                Create_at = x.Created_At,
                Update_at = x.Updated_At
            })
            .Skip(skip)
            .Take((int)pageResult)
            .ToListAsync();

        return Ok(new
        {
            Total = _context.VehicleBrands.Count(),
            Limit = pageResult,
            Skip = skip,
            Data = vehicleTypes
        });
    }
    
    [HttpPost]
    [Authorize("Admin")]
    public async Task<IActionResult> AddVehicleType([FromBody] VehicleTypeDto dto)
    {

        if (dto.Name is null || dto.Brand_id <= 0)
        {
            return BadRequest("Invalid Parameters");
        }

        var brandExists = await _context.VehicleBrands
            .FindAsync(dto.Brand_id);
        if (brandExists is null)
        {
            return NotFound("Brand not found");
        }

        var typeExists = await _context.VehicleTypes
            .Where(x => x.Name.ToLower() == dto.Name.ToLower() && x.BrandId == dto.Brand_id)
            .FirstOrDefaultAsync();

        if (typeExists is not null)
            return Conflict("Type already used in brand id");

        var type = new VehicleType { Name = dto.Name, VehicleBrand = brandExists };
        _context.VehicleTypes.Add(type);
        _context.SaveChanges();
        return Ok("Type created");
    }
    
    [HttpPatch("{id:int}")]
    [Authorize("Admin")]
    public async Task<IActionResult> UpdateVehicleType(int id, [FromBody]VehicleTypeDto dto)
    {
        if (dto.Name is null || dto.Brand_id <= 0)
            return BadRequest();

        var vehicleType = await _context.VehicleTypes.FindAsync(id);
        if (vehicleType is null)
            return NotFound("Vehicle type not found");

        var vehicleBrand = await _context.VehicleBrands.FindAsync(dto.Brand_id);
        if (vehicleBrand is null)
            return NotFound("Vehicle brand not found");

        var typeExists = await _context.VehicleTypes
            .Where(x => x.Name.ToLower() == dto.Name.ToLower() && x.BrandId == dto.Brand_id)
            .FirstOrDefaultAsync();

        if (typeExists is not null)
            return Conflict("Type already used in brand id");

        vehicleType.Name = dto.Name;
        vehicleType.VehicleBrand = vehicleBrand;
        vehicleType.Updated_At = DateTime.UtcNow;
        _context.VehicleTypes.Update(vehicleType);
        _context.SaveChanges();
        return Ok("Type updated");
    }
    
    [HttpDelete("{id:int}")]
    [Authorize("Admin")]
    public async Task<IActionResult> DeleteVehicleType(int id)
    {
        var vehicleType = await _context.VehicleTypes.FindAsync(id);
        if (vehicleType is null)
            return NotFound("Vehicle type not found");
        _context.VehicleTypes.Remove(vehicleType);
        _context.SaveChanges();
        return Ok("Type deleted");
    }
    
}