using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seru.BackendTest.Data;
using Seru.BackendTest.Models;

namespace Seru.BackendTest.Controllers;

[ApiController]
[Route("vehicle-years")]
[Authorize]
public sealed class VehicleYearsController : ControllerBase
{
    private readonly AppDbContext _context;
    public VehicleYearsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllVehicleYears()
    {
        var vehicleYears = await _context.VehicleYears
            .Select(x => new
            {
                Id = x.Id,
                Year = x.Year,
                Create_at = x.Created_At,
                Update_at = x.Updated_At,
            }).OrderBy(x => x.Year).ToListAsync();

        return Ok(vehicleYears);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetVehicleYear(int id)
    {
        var vehicleYear = await _context.VehicleYears.FindAsync(id);
        if (vehicleYear is null)
            return NotFound("Year not found");

        return Ok(new
        {
            Id = vehicleYear.Id,
            Year = vehicleYear.Year,
            Create_at = vehicleYear.Created_At,
            Update_at = vehicleYear.Updated_At
        });
    }
    [HttpGet("page/{page:int}")]
    public async Task<IActionResult> GetVehicleTypes(int page = 1)
    {
        if (page <= 0)
        {
            return BadRequest("Invalid Parameter");
        }
        if (_context.VehicleYears == null)
            return NotFound("Year not found");

        var pageResult = 10;
        var pageCount = Math.Ceiling(_context.VehicleYears.Count() / (float)pageResult);
        var skip = (page - 1) * (int)pageResult;
        var vehicleYears = await _context.VehicleYears
            .Select(x => new
            {
                Id = x.Id,
                Year = x.Year,
                Create_at = x.Created_At,
                Update_at = x.Updated_At
            })
            .OrderBy(x => x.Year)
            .Skip(skip)
            .Take((int)pageResult)
            .ToListAsync();

        return Ok(new
        {
            Total = _context.VehicleBrands.Count(),
            Limit = pageResult,
            Skip = skip,
            Data = vehicleYears
        });
    }
    [HttpPost]
    [Authorize("Admin")]
    public async Task<IActionResult> AddVehicleYear([FromBody] VehicleYearDto dto)
    {
        if (dto.Year is null)
            return BadRequest("Invalid Parameters");

        var yearExists = await _context.VehicleYears
            .SingleOrDefaultAsync(x => x.Year == dto.Year);
        if (yearExists is not null)
            return Conflict("Year already exists");

        var year = new VehicleYear { Year = dto.Year };
        _context.VehicleYears.Add(year);
        _context.SaveChanges();
        return Ok("Year added");
    }
    [HttpPatch("{id:int}")]
    [Authorize("Admin")]
    public async Task<IActionResult> UpdateVehicleYear(int id, [FromBody] VehicleYearDto dto)
    {
        if (dto.Year is null || id <= 0)
            return BadRequest("Invalid Parameters");

        var year = await _context.VehicleYears.FindAsync(id);
        if (year is null)
            return NotFound("Year not found");

        var yearExists = await _context.VehicleYears
            .SingleOrDefaultAsync(x => x.Year == dto.Year);
        if (yearExists is not null)
            return Conflict("Year already in use");

        year.Year = dto.Year;
        _context.VehicleYears.Update(year);
        _context.SaveChanges();

        return Ok("Update success");
    }
    [HttpDelete("{id:int}")]
    [Authorize("Admin")]
    public async Task<IActionResult> DeleteVehicleYear(int id)
    {
        if (id <= 0)
            return BadRequest("Invalid Parameter");

        var year = await _context.VehicleYears.FindAsync(id);
        if (year is null) 
            return NotFound("Year not found");

        _context.VehicleYears.Remove(year);
        _context.SaveChanges();
        return Ok("Year deleted");
    }
}