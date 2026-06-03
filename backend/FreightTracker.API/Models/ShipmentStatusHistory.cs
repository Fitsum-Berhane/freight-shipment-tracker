namespace FreightTracker.API.Models;

public class ShipmentStatusHistory
{
    public int Id { get; set; }
    public int ShipmentId { get; set; }
    public ShipmentStatus Status { get; set; }
    public DateTime ChangedAt { get; set; }
    public Shipment? Shipment { get; set; }
}
