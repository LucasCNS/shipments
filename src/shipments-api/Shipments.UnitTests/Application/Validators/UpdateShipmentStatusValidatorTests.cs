using Xunit;
using Shipments.Application.Validators;

namespace Shipments.UnitTests.Application.Validators;

/// <summary>
/// Unit tests for the UpdateShipmentStatusValidator class.
/// </summary>
public class UpdateShipmentStatusValidatorTests
{
    // ==================== Valid Statuses ====================

    /// <summary>
    /// Test: "pending" is a valid status and should return null.
    /// </summary>
    [Fact]
    public void Validate_PendingStatus_ShouldReturnNull()
    {
        // Act
        var result = UpdateShipmentStatusValidator.Validate("pending");

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Test: "in_transit" is a valid status and should return null.
    /// </summary>
    [Fact]
    public void Validate_InTransitStatus_ShouldReturnNull()
    {
        // Act
        var result = UpdateShipmentStatusValidator.Validate("in_transit");

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Test: "delivered" is a valid status and should return null.
    /// </summary>
    [Fact]
    public void Validate_DeliveredStatus_ShouldReturnNull()
    {
        // Act
        var result = UpdateShipmentStatusValidator.Validate("delivered");

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Test: "cancelled" is a valid status and should return null.
    /// </summary>
    [Fact]
    public void Validate_CancelledStatus_ShouldReturnNull()
    {
        // Act
        var result = UpdateShipmentStatusValidator.Validate("cancelled");

        // Assert
        Assert.Null(result);
    }

    // ==================== Invalid Statuses ====================

    /// <summary>
    /// Test: Invalid status value should return error.
    /// </summary>
    [Fact]
    public void Validate_InvalidStatus_ShouldReturnError()
    {
        // Act
        var result = UpdateShipmentStatusValidator.Validate("invalid");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("VALIDATION_ERROR", result!.Code);
        Assert.Equal(400, result.CorrespondingStatusCode);
        Assert.True(result.ValidationErrors.Count > 0);
    }

    /// <summary>
    /// Test: Another invalid status value should return error.
    /// </summary>
    [Fact]
    public void Validate_ShippingStatus_ShouldReturnError()
    {
        // Act
        var result = UpdateShipmentStatusValidator.Validate("shipping");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("VALIDATION_ERROR", result!.Code);
        Assert.Equal(400, result.CorrespondingStatusCode);
    }

    /// <summary>
    /// Test: Invalid status "completed" should return error.
    /// </summary>
    [Fact]
    public void Validate_CompletedStatus_ShouldReturnError()
    {
        // Act
        var result = UpdateShipmentStatusValidator.Validate("completed");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("VALIDATION_ERROR", result!.Code);
        Assert.Equal(400, result.CorrespondingStatusCode);
    }

    // ==================== Null/Empty Cases ====================

    /// <summary>
    /// Test: Null status should return error.
    /// </summary>
    [Fact]
    public void Validate_NullStatus_ShouldReturnError()
    {
        // Act
        var result = UpdateShipmentStatusValidator.Validate(null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("VALIDATION_ERROR", result!.Code);
        Assert.Equal(400, result.CorrespondingStatusCode);
        Assert.Equal("Status is required and cannot be empty.", result.Message);
    }

    /// <summary>
    /// Test: Empty string status should return error.
    /// </summary>
    [Fact]
    public void Validate_EmptyStatus_ShouldReturnError()
    {
        // Act
        var result = UpdateShipmentStatusValidator.Validate("");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("VALIDATION_ERROR", result!.Code);
        Assert.Equal(400, result.CorrespondingStatusCode);
    }

    /// <summary>
    /// Test: Whitespace-only status should return error.
    /// </summary>
    [Fact]
    public void Validate_WhitespaceStatus_ShouldReturnError()
    {
        // Act
        var result = UpdateShipmentStatusValidator.Validate("   ");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("VALIDATION_ERROR", result!.Code);
        Assert.Equal(400, result.CorrespondingStatusCode);
    }
}
