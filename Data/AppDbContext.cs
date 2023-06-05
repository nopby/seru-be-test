using Microsoft.EntityFrameworkCore;
using Seru.BackendTest.Models;

namespace Seru.BackendTest.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<VehicleBrand> VehicleBrands { get; set; }
    public DbSet<VehicleModel> VehicleModels { get; set; }
    public DbSet<VehicleType> VehicleTypes { get; set; }
    public DbSet<VehicleYear> VehicleYears { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<VehicleBrand>()
            .HasMany(x => x.Types)
            .WithOne(y => y.VehicleBrand)
            .HasForeignKey(y => y.BrandId);

        builder.Entity<VehicleType>()
            .HasMany(x => x.VehicleModels)
            .WithOne(y => y.VehicleType)
            .HasForeignKey(y => y.TypeId);

        builder.Entity<PriceList>()
            .HasOne(x => x.VehicleModel)
            .WithMany(y => y.Prices)
            .HasForeignKey(x => x.ModelId);

        builder.Entity<PriceList>()
            .HasOne(x => x.VehicleYear)
            .WithMany(y => y.Prices)
            .HasForeignKey(y => y.YearId);


        base.OnModelCreating(builder);
    }
}