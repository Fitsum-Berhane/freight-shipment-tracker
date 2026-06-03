using FreightTracker.API.Data;
using FreightTracker.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FreightTracker.API.Repositories;

public class ShipmentRepository : IShipmentRepository
{
    private readonly AppDbContext _context;

    public ShipmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Shipment>> GetAllAsync(ShipmentStatus? status, string? carrier, string? search)
    {
        IQueryable<Shipment> query = _context.Shipments;

        if (status is not null)
            query = query.Where(s => s.Status == status);

        if (!string.IsNullOrWhiteSpace(carrier))
            query = query.Where(s => s.Carrier == carrier);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.ToLower();
            query = query.Where(s =>
                s.Origin.ToLower().Contains(term) || s.Destination.ToLower().Contains(term));
        }

        return await query.OrderByDescending(s => s.CreatedAt).ToListAsync();
    }

    public async Task<Shipment?> GetByIdAsync(int id)
    {
        return await _context.Shipments.FindAsync(id);
    }

    public async Task<Shipment> AddAsync(Shipment shipment)
    {
        _context.Shipments.Add(shipment);
        await _context.SaveChangesAsync();
        return shipment;
    }

    public async Task UpdateAsync(Shipment shipment)
    {
        _context.Shipments.Update(shipment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Shipment shipment)
    {
        _context.Shipments.Remove(shipment);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> TrackingNumberExistsAsync(string trackingNumber)
    {
        return await _context.Shipments.AnyAsync(s => s.TrackingNumber == trackingNumber);
    }

    public async Task<int> CountAsync()
    {
        return await _context.Shipments.CountAsync();
    }

    public async Task<List<ShipmentStatusHistory>> GetStatusHistoryAsync(int shipmentId)
    {
        return await _context.StatusHistory
            .Where(h => h.ShipmentId == shipmentId)
            .OrderBy(h => h.ChangedAt)
            .ToListAsync();
    }
}
