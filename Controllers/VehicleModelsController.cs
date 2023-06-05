using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seru.BackendTest.Data;
using Seru.BackendTest.Models;

namespace Seru.BackendTest.Controllers;

[ApiController]
[Route("vehicle-models")]
[Authorize]
public sealed class VehicleModelsController : ControllerBase
{
    private readonly AppDbContext _context;

    public VehicleModelsController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<IActionResult> GetAllVehicleModels([FromQuery] int? type_id)
    {
        IQueryable<VehicleModel> query = _context.VehicleModels.AsQueryable();
        if (type_id.HasValue)
        {
            query = query.Where(x => x.TypeId == type_id);
        }
        var vehicleModels = await query.Select(x => new
        {
            Id = x.Id,
            Name = x.Name,
            Type_id = x.TypeId,
            Create_at = x.Created_At,
            Update_at = x.Updated_At,
        }).ToListAsync();
        return Ok(vehicleModels);
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetVehicleModel(int id)
    {
        var vehicleModel = await _context.VehicleModels.FindAsync(id);
        if (vehicleModel is null)
            return NotFound();

        return Ok(new
        {
            Id = vehicleModel.Id,
            Name = vehicleModel.Name,
            Type_id = vehicleModel.TypeId,
            Create_at = vehicleModel.Created_At,
            Update_at = vehicleModel.Updated_At,
        });
    }
    
    [HttpGet("page/{page:int}")]
    public async Task<IActionResult> GetVehicleTypes(int page = 1)
    {
        if (page <= 0)
        {
            return BadRequest();
        }
        if (_context.VehicleModels == null)
            return NotFound();

        var pageResult = 10;
        var pageCount = Math.Ceiling(_context.VehicleModels.Count() / (float)pageResult);
        var skip = (page - 1) * (int)pageResult;
        var vehicleTypes = await _context.VehicleModels
            .Select(x => new
            {
                Id = x.Id,
                Name = x.Name,
                Type_id = x.TypeId,
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
            Data = vehicleTypes
        });
    }
    
    [HttpPost]
    [Authorize("Admin")]
    public async Task<IActionResult> AddVehicleModel([FromBody] VehicleModelDto dto)
    {
        if (dto.Name is null || dto.Type_id <= 0)
            return BadRequest("Invalid Parameters");

        var typeExists = await _context.VehicleTypes.FindAsync(dto.Type_id);
        if (typeExists is null)
            return NotFound("Type not found");

        var modelExists = await _context.VehicleModels
            .SingleOrDefaultAsync(x => x.Name == dto.Name && x.TypeId == dto.Type_id);
        if (modelExists is not null)
            return Conflict("Model already exists in type");

        var vehicleModels = new VehicleModel { Name = dto.Name, TypeId = dto.Type_id };
        _context.VehicleModels.Add(vehicleModels);
        _context.SaveChanges();

        return Ok("Model created");
    }
    
    [HttpPatch("{id:int}")]
    [Authorize("Admin")]
    public async Task<IActionResult> UpdateVehicleModel(int id, [FromBody] VehicleModelDto dto)
    {
        if (dto.Name is null || dto.Type_id <= 0 || id <= 0)
            return BadRequest("Invalid Paramaters");

        var modelExists = await _context.VehicleModels.FindAsync(id);
        if (modelExists is null)
            return NotFound("Model not found");

        var typeExists = await _context.VehicleTypes.FindAsync(dto.Type_id);
        if (typeExists is null)
            return NotFound("Type not found");

        var modelInType = await _context.VehicleModels
            .SingleOrDefaultAsync(x => x.Name == dto.Name && x.TypeId == dto.Type_id);
        if (modelInType is not null)
            return Conflict("Model already exists in type");

        modelExists.Updated_At = DateTime.UtcNow;
        modelExists.Name = dto.Name;
        modelExists.VehicleType = typeExists;
            
        _context.VehicleModels.Update(modelExists);
        _context.SaveChanges();

        return Ok("Model updated");
    }
    
    [HttpDelete("{id:int}")]
    [Authorize("Admin")]
    public async Task<IActionResult> DeleteVehicleModel(int id)
    {
        var vehicleModel = await _context.VehicleModels.FindAsync(id);
        if (vehicleModel is null)
            return NotFound();

        _context.VehicleModels.Remove(vehicleModel);
        _context.SaveChanges();

        return Ok("Model deleted");
    }
}