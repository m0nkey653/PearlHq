using Microsoft.EntityFrameworkCore;
using PearlHqWeb.Models;

namespace PearlHqWeb.Data;

public class PearlHqDb : DbContext
{
    public PearlHqDb(DbContextOptions<PearlHqDb> options)
        : base(options)
    {
    }

    public DbSet<Device> Devices { get; set; } = default!;
    public DbSet<Dish> Dishes { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Device>().ToTable("Device");
        modelBuilder.Entity<Dish>().ToTable("Dish");
    }
}
