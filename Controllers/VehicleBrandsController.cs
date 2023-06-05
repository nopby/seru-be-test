using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seru.BackendTest.Data;
using Seru.BackendTest.Models;
using System.Reflection.Metadata.Ecma335;

namespace Seru.BackendTest.Controllers;

[ApiController]
[Route("vehicle-brands")]
[Authorize]
public sealed class VehicleBrandsController : ControllerBase
{
    private readonly AppDbContext _context;

    public VehicleBrandsController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<IActionResult> GetAllVehicleBrand()
    {
        var vehicleBrands = await _context.VehicleBrands
            .Select(x => new
            {
                Id = x.Id,
                Name = x.Name,
                Created_at = x.Created_At,
                Update_at = x.Updated_At
            }).ToListAsync();

        return Ok(vehicleBrands);
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetVehicleBrand(int id)
    {
        var vehicleBrand = await _context.VehicleBrands.FindAsync(id);
        if (vehicleBrand is null)
            return NotFound("Brand not found");

        return Ok(new
        {
            Id = vehicleBrand.Id,
            Name = vehicleBrand.Name,
            Created_at = vehicleBrand.Created_At,
            Update_at = vehicleBrand.Updated_At
        });
    }

    [HttpGet("page/{page:int}")]
    public async Task<IActionResult> GetVehicleBrands(int page = 1)
    {
        if (_context.VehicleBrands == null)
            return NotFound("Brand not found");

        var pageResult = 10;
        var pageCount = Math.Ceiling(_context.VehicleBrands.Count() / (float)pageResult);
        var skip = (page - 1) * (int)pageResult;
        var vehicleBrands = await _context.VehicleBrands
            .Select(x => new
            {
                Id = x.Id,
                Name = x.Name,
                Create_at = x.Created_At,
                Update_at = x.Updated_At,
            })
            .Skip(skip)
            .Take((int)pageResult)
            .ToListAsync();

        return Ok(new
        {
            Total = _context.VehicleBrands.Count(),
            Limit = pageResult,
            Skip = skip,
            Data = vehicleBrands
        });
    }

    [HttpPost]
    [Authorize("Admin")]
    public async Task<IActionResult> AddVehicleBrand([FromBody] VehicleBrandDto dto)
    {
        if (dto.Name == null)
            return BadRequest("Invalid Parameter");

        var exists = await _context.VehicleBrands.SingleOrDefaultAsync(x => x.Name == dto.Name);
        if (exists is not null)
        {
            return Conflict("Brand already exists");
        }

        var newVehicleBrand = new VehicleBrand { Name = dto.Name };
        _context.VehicleBrands.Add(newVehicleBrand);
        _context.SaveChanges();
        return Ok("Brand created");
    }
    
    [HttpPatch("{id:int}")]
    [Authorize("Admin")]
    public async Task<IActionResult> UpdateVehicleBrand(int id, [FromBody] VehicleBrandDto dto)
    {
        if (dto.Name is null)
            return BadRequest();

        var exists = await _context.VehicleBrands.FindAsync(id);
        if (exists == null)
        {
            return NotFound("Brand not found");
        }

        exists.Name = dto.Name;
        exists.Updated_At = DateTime.UtcNow;
        _context.VehicleBrands.Update(exists);
        _context.SaveChanges();
        return Ok("Brand updated");
    }
    [HttpDelete("{id:int}")]
    [Authorize("Admin")]
    public async Task<IActionResult> DeleteVehicleBrand(int id)
    {
        var exists = await _context.VehicleBrands.FindAsync(id);
        if (exists == null)
        {
            return NotFound("Brand not found");
        }
        _context.VehicleBrands.Remove(exists);
        _context.SaveChanges();
        return Ok("Brand deleted");
    }
    
}