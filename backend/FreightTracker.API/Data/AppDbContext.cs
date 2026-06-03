using FreightTracker.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FreightTracker.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<ShipmentStatusHistory> StatusHistory => Set<ShipmentStatusHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Shipment>(entity =>
        {
            entity.Property(s => s.TrackingNumber).IsRequired().HasMaxLength(40);
            entity.HasIndex(s => s.TrackingNumber).IsUnique();

            entity.Property(s => s.Origin).IsRequired().HasMaxLength(100);
            entity.Property(s => s.Destination).IsRequired().HasMaxLength(100);
            entity.Property(s => s.Carrier).IsRequired().HasMaxLength(100);

            // SQLite has no decimal type, so keep precision by storing as text
            entity.Property(s => s.WeightKg).HasColumnType("TEXT");

            entity.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);
        });

        modelBuilder.Entity<ShipmentStatusHistory>(entity =>
        {
            entity.Property(h => h.Status).HasConversion<string>().HasMaxLength(20);

            entity.HasOne(h => h.Shipment)
                .WithMany(s => s.StatusHistory)
                .HasForeignKey(h => h.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
