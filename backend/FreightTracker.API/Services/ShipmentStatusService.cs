using FreightTracker.API.Models;

namespace FreightTracker.API.Services;

public class ShipmentStatusService : IShipmentStatusService
{
    private static readonly IReadOnlyDictionary<ShipmentStatus, ShipmentStatus[]> Transitions =
        new Dictionary<ShipmentStatus, ShipmentStatus[]>
        {
            [ShipmentStatus.Pending] = new[] { ShipmentStatus.InTransit, ShipmentStatus.Cancelled },
            [ShipmentStatus.InTransit] = new[] { ShipmentStatus.Delivered, ShipmentStatus.Cancelled },
            [ShipmentStatus.Delivered] = Array.Empty<ShipmentStatus>(),
            [ShipmentStatus.Cancelled] = Array.Empty<ShipmentStatus>()
        };

    public bool CanTransition(ShipmentStatus from, ShipmentStatus to)
    {
        return Transitions.TryGetValue(from, out var allowed) && allowed.Contains(to);
    }

    public IReadOnlyList<ShipmentStatus> GetAllowedTransitions(ShipmentStatus from)
    {
        return Transitions.TryGetValue(from, out var allowed) ? allowed : Array.Empty<ShipmentStatus>();
    }
}
