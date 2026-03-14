using Xunit;
using Shipments.Application.StateTransitions;

namespace Shipments.UnitTests.StateTransitions;

/// <summary>
/// Unit tests for the ShipmentStateTransitionValidator class.
/// </summary>
public class ShipmentStateTransitionValidatorTests
{
    // ===== Valid transitions =====

    [Fact]
    public void Validate_PendingToInTransit_ShouldReturnNull()
    {
        var result = ShipmentStateTransitionValidator.Validate("pending", "in_transit");
        Assert.Null(result);
    }

    [Fact]
    public void Validate_PendingToCancelled_ShouldReturnNull()
    {
        var result = ShipmentStateTransitionValidator.Validate("pending", "cancelled");
        Assert.Null(result);
    }

    [Fact]
    public void Validate_InTransitToDelivered_ShouldReturnNull()
    {
        var result = ShipmentStateTransitionValidator.Validate("in_transit", "delivered");
        Assert.Null(result);
    }

    [Fact]
    public void Validate_InTransitToCancelled_ShouldReturnNull()
    {
        var result = ShipmentStateTransitionValidator.Validate("in_transit", "cancelled");
        Assert.Null(result);
    }

    // ===== Invalid transitions =====

    [Fact]
    public void Validate_PendingToDelivered_ShouldReturnError()
    {
        var result = ShipmentStateTransitionValidator.Validate("pending", "delivered");

        Assert.NotNull(result);
        Assert.Equal("INVALID_STATUS_TRANSITION", result.Code);
        Assert.Equal(409, result.CorrespondingStatusCode);
    }

    [Fact]
    public void Validate_InTransitToPending_ShouldReturnError()
    {
        var result = ShipmentStateTransitionValidator.Validate("in_transit", "pending");

        Assert.NotNull(result);
        Assert.Equal("INVALID_STATUS_TRANSITION", result.Code);
        Assert.Equal(409, result.CorrespondingStatusCode);
    }

    [Theory]
    [InlineData("delivered", "pending")]
    [InlineData("delivered", "in_transit")]
    [InlineData("delivered", "cancelled")]
    public void Validate_DeliveredToAnyStatus_ShouldReturnFinalStateError(string current, string target)
    {
        var result = ShipmentStateTransitionValidator.Validate(current, target);

        Assert.NotNull(result);
        Assert.Equal("INVALID_STATUS_TRANSITION", result.Code);
        Assert.Equal(409, result.CorrespondingStatusCode);
        Assert.Contains("final state", result.Message);
    }

    [Theory]
    [InlineData("cancelled", "pending")]
    [InlineData("cancelled", "in_transit")]
    [InlineData("cancelled", "delivered")]
    public void Validate_CancelledToAnyStatus_ShouldReturnFinalStateError(string current, string target)
    {
        var result = ShipmentStateTransitionValidator.Validate(current, target);

        Assert.NotNull(result);
        Assert.Equal("INVALID_STATUS_TRANSITION", result.Code);
        Assert.Equal(409, result.CorrespondingStatusCode);
        Assert.Contains("final state", result.Message);
    }

    [Theory]
    [InlineData("pending", "pending")]
    [InlineData("in_transit", "in_transit")]
    [InlineData("delivered", "delivered")]
    [InlineData("cancelled", "cancelled")]
    public void Validate_SameStatus_ShouldReturnError(string status, string sameStatus)
    {
        var result = ShipmentStateTransitionValidator.Validate(status, sameStatus);

        Assert.NotNull(result);
        Assert.Equal("INVALID_STATUS_TRANSITION", result.Code);
        Assert.Equal(409, result.CorrespondingStatusCode);
    }

    // ===== IsValidStatus tests =====

    [Theory]
    [InlineData("pending")]
    [InlineData("in_transit")]
    [InlineData("delivered")]
    [InlineData("cancelled")]
    public void IsValidStatus_ValidStatuses_ShouldReturnTrue(string status)
    {
        Assert.True(ShipmentStateTransitionValidator.IsValidStatus(status));
    }

    [Theory]
    [InlineData("unknown")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("PENDING")]
    [InlineData("shipped")]
    public void IsValidStatus_InvalidStatuses_ShouldReturnFalse(string? status)
    {
        Assert.False(ShipmentStateTransitionValidator.IsValidStatus(status));
    }
}
