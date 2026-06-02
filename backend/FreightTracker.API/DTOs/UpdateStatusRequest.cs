using FreightTracker.API.Models;

namespace FreightTracker.API.DTOs;

// What a client sends to PATCH a shipment's status.
// Just the target status — the service will decide if the transition is allowed.
public class UpdateStatusRequest
{
    public ShipmentStatus Status { get; set; }
}
