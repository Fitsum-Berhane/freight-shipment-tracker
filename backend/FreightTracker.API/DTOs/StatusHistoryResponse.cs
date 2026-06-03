using FreightTracker.API.Models;

namespace FreightTracker.API.DTOs;

public class StatusHistoryResponse
{
    public ShipmentStatus Status { get; set; }
    public DateTime ChangedAt { get; set; }
}
