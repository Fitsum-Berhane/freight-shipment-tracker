using FreightTracker.API.Models;

namespace FreightTracker.API.DTOs;

// What the API sends BACK to the client.
// Includes the server-controlled fields (Id, TrackingNumber, Status, CreatedAt)
// that the request DTOs deliberately leave out.
public class ShipmentResponse
{
    public int Id { get; set; }
    public string TrackingNumber { get; set; } = "";
    public string Origin { get; set; } = "";
    public string Destination { get; set; } = "";
    public string Carrier { get; set; } = "";
    public decimal WeightKg { get; set; }
    public ShipmentStatus Status { get; set; }
    public DateTime? EstimatedDelivery { get; set; }
    public DateTime CreatedAt { get; set; }
}
