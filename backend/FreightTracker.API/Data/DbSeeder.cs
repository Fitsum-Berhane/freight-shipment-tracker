using FreightTracker.API.Models;

namespace FreightTracker.API.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext context)
    {
        if (context.Shipments.Any())
            return;

        var now = DateTime.UtcNow;

        context.Shipments.AddRange(
            new Shipment
            {
                TrackingNumber = "FRT-2026-0001",
                Origin = "Hamburg",
                Destination = "Boston",
                Carrier = "Maersk",
                WeightKg = 1200.5m,
                Status = ShipmentStatus.Pending,
                EstimatedDelivery = now.AddDays(12),
                CreatedAt = now.AddDays(-2),
                StatusHistory =
                {
                    new ShipmentStatusHistory { Status = ShipmentStatus.Pending, ChangedAt = now.AddDays(-2) },
                }
            },
            new Shipment
            {
                TrackingNumber = "FRT-2026-0002",
                Origin = "Shanghai",
                Destination = "Rotterdam",
                Carrier = "DHL",
                WeightKg = 845.0m,
                Status = ShipmentStatus.InTransit,
                EstimatedDelivery = now.AddDays(5),
                CreatedAt = now.AddDays(-6),
                StatusHistory =
                {
                    new ShipmentStatusHistory { Status = ShipmentStatus.Pending, ChangedAt = now.AddDays(-6) },
                    new ShipmentStatusHistory { Status = ShipmentStatus.InTransit, ChangedAt = now.AddDays(-5) },
                }
            },
            new Shipment
            {
                TrackingNumber = "FRT-2026-0003",
                Origin = "Memphis",
                Destination = "Chicago",
                Carrier = "FedEx Freight",
                WeightKg = 320.75m,
                Status = ShipmentStatus.Delivered,
                EstimatedDelivery = now.AddDays(-1),
                CreatedAt = now.AddDays(-9),
                StatusHistory =
                {
                    new ShipmentStatusHistory { Status = ShipmentStatus.Pending, ChangedAt = now.AddDays(-9) },
                    new ShipmentStatusHistory { Status = ShipmentStatus.InTransit, ChangedAt = now.AddDays(-7) },
                    new ShipmentStatusHistory { Status = ShipmentStatus.Delivered, ChangedAt = now.AddDays(-1) },
                }
            },
            new Shipment
            {
                TrackingNumber = "FRT-2026-0004",
                Origin = "Los Angeles",
                Destination = "Tokyo",
                Carrier = "Maersk",
                WeightKg = 2050.0m,
                Status = ShipmentStatus.Cancelled,
                EstimatedDelivery = null,
                CreatedAt = now.AddDays(-4),
                StatusHistory =
                {
                    new ShipmentStatusHistory { Status = ShipmentStatus.Pending, ChangedAt = now.AddDays(-4) },
                    new ShipmentStatusHistory { Status = ShipmentStatus.Cancelled, ChangedAt = now.AddDays(-3) },
                }
            }
        );

        context.SaveChanges();
    }
}
