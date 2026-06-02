using System.ComponentModel.DataAnnotations;

namespace FreightTracker.API.DTOs;

// What a client sends to UPDATE a shipment's details (PUT).
// Same editable fields as create. Status is NOT here — it has its own PATCH endpoint
// so the transition rule can be enforced separately.
public class UpdateShipmentRequest
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

    [Range(0.01, double.MaxValue, ErrorMessage = "Weight must be greater than 0.")]
    public decimal WeightKg { get; set; }

    public DateTime? EstimatedDelivery { get; set; }
}
