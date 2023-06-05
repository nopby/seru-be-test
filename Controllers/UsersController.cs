using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seru.BackendTest.Data;
using Seru.BackendTest.Models;
using System.Text.RegularExpressions;

namespace Seru.BackendTest.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
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
        // Mapping
        var data = await _context.Users.Select(x => new
        {
            Id = x.Id,
            Name = x.Name,
            Email = x.Email,
            IsAdmin = x.IsAdmin,
            CreatedAt = x.Created_At,
            UpdatedAt = x.Updated_At,
        }).ToListAsync();
        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _context.Users.FindAsync(id);
        return Ok(new
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            IsAdmin = user.IsAdmin,
            CreatedAt = user.Created_At,
            UpdatedAt = user.Updated_At,
        });
    }

    [HttpPost]
    [Authorize("Admin")]
    public async Task<IActionResult> CreateUser([FromBody] UserDto dto)
    {
        // Cek input
        string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
        Regex regex = new Regex(pattern);
        if (dto.Email is null || 
            dto.Password is null ||
            dto.Name is null ||
            !regex.IsMatch(dto.Email))
            return BadRequest("Invalid Parameters");

        // Cek duplikat
        var exists = await _context.Users.SingleOrDefaultAsync(x => x.Email == dto.Email);
        if (exists is not null)
            return Conflict("Email already in use");

        // Tambah ke database
        var user = new User
        {
            Email = dto.Email,
            Password = dto.Password,
            Name = dto.Name,
            IsAdmin = dto.IsAdmin,
        };

        _context.Users.Add(user);
        _context.SaveChanges();
        return Ok("User created");
    }
    
    [HttpPatch("{id:int}")]
    [Authorize("Admin")]
    public async Task<IActionResult> PatchUser(int id, [FromBody] UserDto dto)
    {
        if (id <= 0 || dto.Email is null || dto.Password is null || dto.Name is null)
            return BadRequest("Invalid Parameters");

        var user = await _context.Users.FindAsync(id);
        if (user is null)
            return NotFound("User not found");

        var emailExists = await _context.Users
            .SingleOrDefaultAsync(x => x.Email == dto.Email);
        if (emailExists is not null)
            return Conflict("Email already in use");

        user.Name = dto.Name;
        user.Email = dto.Email;
        user.Password = dto.Password;
        user.IsAdmin = dto.IsAdmin;
        user.Updated_At = DateTime.UtcNow;

        _context.Users.Update(user);
        _context.SaveChanges();
        return Ok("User updated"); 
    }
    
    [HttpDelete("{id:int}")]
    [Authorize("Admin")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null)
            return NotFound("User not found");
        _context.Users.Remove(user);
        _context.SaveChanges();
        return Ok("User deleted");
    }
    
    [HttpGet("page/{page:int}")]
    public async Task<IActionResult> GetUsers(int page = 1)
    {
        if (_context.Users == null)
            return NotFound("User not found");

        var pageResult = 10;
        var pageCount = Math.Ceiling(_context.Users.Count() / (float)pageResult);
        var skip = (page - 1) * (int)pageResult;
        var users = await _context.Users
            .Select(x => new
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                IsAdmin = x.IsAdmin,
                CreatedAt = x.Created_At,
                UpdatedAt = x.Updated_At,
            })
            .Skip(skip)
            .Take((int)pageResult)
            .ToListAsync();

        return Ok(new
        {
            Total = _context.Users.Count(),
            Limit = pageResult,
            Skip = skip,
            Data = users
        });
    }
}