namespace FreightTracker.API.Models;

public enum ShipmentStatus
{
    Pending,
    InTransit,
    Delivered,
    Cancelled
}

public class Shipment
{
    public int Id { get; set; }
    public string TrackingNumber { get; set; } = "";
    public string Origin { get; set; } = "";
    public string Destination { get; set; } = "";
    public string Carrier { get; set; } = "";
    public decimal WeightKg { get; set; }
    public ShipmentStatus Status { get; set; } = ShipmentStatus.Pending;
    public DateTime? EstimatedDelivery { get; set; }
    public DateTime CreatedAt { get; set; }
}
