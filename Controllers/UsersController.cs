using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seru.BackendTest.Data;
using Seru.BackendTest.Data.Dtos;
using Seru.BackendTest.Models;

namespace Seru.BackendTest.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    public UsersController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var data = await _context.Users.ToListAsync();
        return Ok(data);
    }
    [HttpPost]
    public async Task<IActionResult> AddUser(AddUserDto dto)
    {
        if (dto.Email is null && dto.Password is null)
            return BadRequest();

        var exists = _context.Users.Where(u => u.Email == dto.Email).Any();
        if (exists)
            return Conflict();


        return Ok();
    }
}