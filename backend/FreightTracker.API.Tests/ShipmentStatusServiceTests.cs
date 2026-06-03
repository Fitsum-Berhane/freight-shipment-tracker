using FreightTracker.API.Models;
using FreightTracker.API.Services;
using Xunit;

namespace FreightTracker.API.Tests;

public class ShipmentStatusServiceTests
{
    private readonly ShipmentStatusService _service = new();

    [Theory]
    [InlineData(ShipmentStatus.Pending, ShipmentStatus.InTransit)]
    [InlineData(ShipmentStatus.Pending, ShipmentStatus.Cancelled)]
    [InlineData(ShipmentStatus.InTransit, ShipmentStatus.Delivered)]
    [InlineData(ShipmentStatus.InTransit, ShipmentStatus.Cancelled)]
    public void CanTransition_AllowsValidMoves(ShipmentStatus from, ShipmentStatus to)
    {
        Assert.True(_service.CanTransition(from, to));
    }

    [Theory]
    [InlineData(ShipmentStatus.Pending, ShipmentStatus.Delivered)]
    [InlineData(ShipmentStatus.Pending, ShipmentStatus.Pending)]
    [InlineData(ShipmentStatus.InTransit, ShipmentStatus.Pending)]
    [InlineData(ShipmentStatus.InTransit, ShipmentStatus.InTransit)]
    [InlineData(ShipmentStatus.Delivered, ShipmentStatus.Pending)]
    [InlineData(ShipmentStatus.Delivered, ShipmentStatus.InTransit)]
    [InlineData(ShipmentStatus.Delivered, ShipmentStatus.Cancelled)]
    [InlineData(ShipmentStatus.Delivered, ShipmentStatus.Delivered)]
    [InlineData(ShipmentStatus.Cancelled, ShipmentStatus.Pending)]
    [InlineData(ShipmentStatus.Cancelled, ShipmentStatus.InTransit)]
    [InlineData(ShipmentStatus.Cancelled, ShipmentStatus.Delivered)]
    [InlineData(ShipmentStatus.Cancelled, ShipmentStatus.Cancelled)]
    public void CanTransition_RejectsInvalidMoves(ShipmentStatus from, ShipmentStatus to)
    {
        Assert.False(_service.CanTransition(from, to));
    }

    [Fact]
    public void GetAllowedTransitions_FromPending_ReturnsInTransitAndCancelled()
    {
        var allowed = _service.GetAllowedTransitions(ShipmentStatus.Pending);
        Assert.Equal(new[] { ShipmentStatus.InTransit, ShipmentStatus.Cancelled }, allowed);
    }

    [Fact]
    public void GetAllowedTransitions_FromInTransit_ReturnsDeliveredAndCancelled()
    {
        var allowed = _service.GetAllowedTransitions(ShipmentStatus.InTransit);
        Assert.Equal(new[] { ShipmentStatus.Delivered, ShipmentStatus.Cancelled }, allowed);
    }

    [Theory]
    [InlineData(ShipmentStatus.Delivered)]
    [InlineData(ShipmentStatus.Cancelled)]
    public void GetAllowedTransitions_FromTerminalState_ReturnsEmpty(ShipmentStatus from)
    {
        Assert.Empty(_service.GetAllowedTransitions(from));
    }
}
