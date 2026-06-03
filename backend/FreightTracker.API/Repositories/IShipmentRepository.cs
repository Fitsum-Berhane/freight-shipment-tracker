using FreightTracker.API.Models;

namespace FreightTracker.API.Repositories;

public interface IShipmentRepository
{
    Task<List<Shipment>> GetAllAsync(ShipmentStatus? status, string? carrier, string? search);
    Task<Shipment?> GetByIdAsync(int id);
    Task<Shipment> AddAsync(Shipment shipment);
    Task UpdateAsync(Shipment shipment);
    Task DeleteAsync(Shipment shipment);
    Task<bool> TrackingNumberExistsAsync(string trackingNumber);
    Task<int> CountAsync();
    Task<List<ShipmentStatusHistory>> GetStatusHistoryAsync(int shipmentId);
}
