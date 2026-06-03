using FreightTracker.API.Models;

namespace FreightTracker.API.Services;

public interface IShipmentStatusService
{
    bool CanTransition(ShipmentStatus from, ShipmentStatus to);
    IReadOnlyList<ShipmentStatus> GetAllowedTransitions(ShipmentStatus from);
}
