using FreightTracker.API.DTOs;
using FreightTracker.API.Models;
using FreightTracker.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace FreightTracker.API.Controllers;

[ApiController]
[Route("api/shipments")]
public class ShipmentsController : ControllerBase
{
    private readonly IShipmentService _service;

    public ShipmentsController(IShipmentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<ShipmentResponse>>> GetAll(
        [FromQuery] ShipmentStatus? status,
        [FromQuery] string? carrier,
        [FromQuery] string? search)
    {
        var shipments = await _service.GetAllAsync(status, carrier, search);
        return Ok(shipments);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ShipmentResponse>> GetById(int id)
    {
        var shipment = await _service.GetByIdAsync(id);
        return shipment is null ? NotFound() : Ok(shipment);
    }

    [HttpPost]
    public async Task<ActionResult<ShipmentResponse>> Create(CreateShipmentRequest request)
    {
        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ShipmentResponse>> Update(int id, UpdateShipmentRequest request)
    {
        var updated = await _service.UpdateAsync(id, request);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<ShipmentResponse>> UpdateStatus(int id, UpdateStatusRequest request)
    {
        try
        {
            var updated = await _service.UpdateStatusAsync(id, request.Status);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (InvalidStatusTransitionException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
