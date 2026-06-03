using FreightTracker.API.DTOs;
using FreightTracker.API.Models;
using FreightTracker.API.Repositories;

namespace FreightTracker.API.Services;

public class ShipmentService : IShipmentService
{
    private readonly IShipmentRepository _repository;
    private readonly IShipmentStatusService _statusService;

    public ShipmentService(IShipmentRepository repository, IShipmentStatusService statusService)
    {
        _repository = repository;
        _statusService = statusService;
    }

    public async Task<List<ShipmentResponse>> GetAllAsync(ShipmentStatus? status, string? carrier, string? search)
    {
        var shipments = await _repository.GetAllAsync(status, carrier, search);
        return shipments.Select(ToResponse).ToList();
    }

    public async Task<ShipmentResponse?> GetByIdAsync(int id)
    {
        var shipment = await _repository.GetByIdAsync(id);
        return shipment is null ? null : ToResponse(shipment);
    }

    public async Task<ShipmentResponse> CreateAsync(CreateShipmentRequest request)
    {
        var shipment = new Shipment
        {
            TrackingNumber = await GenerateTrackingNumberAsync(),
            Origin = request.Origin,
            Destination = request.Destination,
            Carrier = request.Carrier,
            WeightKg = request.WeightKg,
            Status = ShipmentStatus.Pending,
            EstimatedDelivery = request.EstimatedDelivery,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(shipment);
        return ToResponse(shipment);
    }

    public async Task<ShipmentResponse?> UpdateAsync(int id, UpdateShipmentRequest request)
    {
        var shipment = await _repository.GetByIdAsync(id);
        if (shipment is null)
            return null;

        shipment.Origin = request.Origin;
        shipment.Destination = request.Destination;
        shipment.Carrier = request.Carrier;
        shipment.WeightKg = request.WeightKg;
        shipment.EstimatedDelivery = request.EstimatedDelivery;

        await _repository.UpdateAsync(shipment);
        return ToResponse(shipment);
    }

    public async Task<ShipmentResponse?> UpdateStatusAsync(int id, ShipmentStatus newStatus)
    {
        var shipment = await _repository.GetByIdAsync(id);
        if (shipment is null)
            return null;

        if (!_statusService.CanTransition(shipment.Status, newStatus))
            throw new InvalidStatusTransitionException(
                $"Cannot change status from {shipment.Status} to {newStatus}.");

        shipment.Status = newStatus;
        await _repository.UpdateAsync(shipment);
        return ToResponse(shipment);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var shipment = await _repository.GetByIdAsync(id);
        if (shipment is null)
            return false;

        await _repository.DeleteAsync(shipment);
        return true;
    }

    private async Task<string> GenerateTrackingNumberAsync()
    {
        var year = DateTime.UtcNow.Year;
        var sequence = await _repository.CountAsync() + 1;

        string candidate;
        do
        {
            candidate = $"FRT-{year}-{sequence:D4}";
            sequence++;
        }
        while (await _repository.TrackingNumberExistsAsync(candidate));

        return candidate;
    }

    private static ShipmentResponse ToResponse(Shipment shipment) => new()
    {
        Id = shipment.Id,
        TrackingNumber = shipment.TrackingNumber,
        Origin = shipment.Origin,
        Destination = shipment.Destination,
        Carrier = shipment.Carrier,
        WeightKg = shipment.WeightKg,
        Status = shipment.Status,
        EstimatedDelivery = shipment.EstimatedDelivery,
        CreatedAt = shipment.CreatedAt
    };
}
