using FreightTracker.API.Models;

namespace FreightTracker.API.DTOs;

public class UpdateStatusRequest
{
    public ShipmentStatus Status { get; set; }
}
