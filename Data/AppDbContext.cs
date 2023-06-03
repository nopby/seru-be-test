using Microsoft.EntityFrameworkCore;
using Seru.BackendTest.Models;

namespace Seru.BackendTest.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
    public DbSet<User> Users { get; set; }
}