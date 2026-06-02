using FreightTracker.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FreightTracker.API.Data;

public class AppDbContext : DbContext
{
    // Program.cs passes in options (the SQLite connection string) via this constructor.
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // "The Shipments table, as a queryable C# collection."
    public DbSet<Shipment> Shipments => Set<Shipment>();

    // Where we configure column rules that EF Core can't guess from the class alone.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Shipment>(entity =>
        {
            entity.Property(s => s.TrackingNumber).IsRequired().HasMaxLength(40);
            entity.HasIndex(s => s.TrackingNumber).IsUnique();   // enforce uniqueness at DB level

            entity.Property(s => s.Origin).IsRequired().HasMaxLength(100);
            entity.Property(s => s.Destination).IsRequired().HasMaxLength(100);
            entity.Property(s => s.Carrier).IsRequired().HasMaxLength(100);

            // SQLite has no native decimal type; store as TEXT to preserve precision.
            entity.Property(s => s.WeightKg).HasColumnType("TEXT");

            // Store the enum as readable text ("Pending") instead of a number (0).
            entity.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);
        });
    }
}
