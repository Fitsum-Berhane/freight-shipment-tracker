using System.ComponentModel.DataAnnotations;

namespace FreightTracker.API.DTOs;

// What a client must send to CREATE a shipment.
// Note what is NOT here: Id, TrackingNumber, Status, CreatedAt — the server owns those.
public class CreateShipmentRequest
{
    [Required]
    [MaxLength(100)]
    public string Origin { get; set; } = "";

    [Required]
    [MaxLength(100)]
    public string Destination { get; set; } = "";

    [Required]
    [MaxLength(100)]
    public string Carrier { get; set; } = "";

    // Must be greater than 0. Range's lower bound enforces that.
    [Range(0.01, double.MaxValue, ErrorMessage = "Weight must be greater than 0.")]
    public decimal WeightKg { get; set; }

    // Optional — the '?' allows it to be left out (null).
    public DateTime? EstimatedDelivery { get; set; }
}
