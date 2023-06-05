using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seru.BackendTest.Data;
using Seru.BackendTest.Models;

namespace Seru.BackendTest.Controllers;

[ApiController]
[Route("pricelist")]
[Authorize]
public sealed class PriceListController : ControllerBase
{
    private readonly AppDbContext _context;

    public PriceListController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<IActionResult> GetPriceList()
    {
        var result = await _context.VehicleYears
            .SelectMany(x => x.Prices)
            .Include(m => m.VehicleModel)
            .Include(y => y.VehicleYear)
            .Select(x => new
            {
                Id = x.Id,
                Year = x.VehicleYear.Year,
                Model = x.VehicleModel.Name,
                Create_at = x.Created_At,
                Update_at = x.Updated_At
            })
            .ToListAsync();
        return Ok(result);
    }
    [HttpPost]
    [Authorize("Admin")]
    public async Task<IActionResult> AddPriceList([FromBody] PriceListDto dto)
    {
        if (dto.Model is null || dto.Year is null)
            return BadRequest("Invalid Parameters");

        var model = await _context.VehicleModels
            .Include(x => x.Prices)
            .SingleOrDefaultAsync(x => x.Name == dto.Model);

        if (model is null)
            return NotFound("Model not found");

        var year = await _context.VehicleYears
            .Include(x => x.Prices)
            .SingleOrDefaultAsync(x => x.Year == dto.Year);
        if (year is null)
            return NotFound("Year not found");

        var pricelist = new PriceList { VehicleModel = model, VehicleYear = year };
        model.Prices.Add(pricelist);
        year.Prices.Add(pricelist);
        model.Updated_At = DateTime.UtcNow;
        year.Updated_At = DateTime.UtcNow;
        _context.VehicleModels.Update(model);
        _context.VehicleYears.Update(year);
        _context.SaveChanges();
        return Ok();
    }
}