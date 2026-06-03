using FreightTracker.API.DTOs;
using FreightTracker.API.Models;

namespace FreightTracker.API.Services;

public interface IShipmentService
{
    Task<List<ShipmentResponse>> GetAllAsync(ShipmentStatus? status, string? carrier, string? search);
    Task<ShipmentResponse?> GetByIdAsync(int id);
    Task<ShipmentResponse> CreateAsync(CreateShipmentRequest request);
    Task<ShipmentResponse?> UpdateAsync(int id, UpdateShipmentRequest request);
    Task<ShipmentResponse?> UpdateStatusAsync(int id, ShipmentStatus newStatus);
    Task<bool> DeleteAsync(int id);
}
