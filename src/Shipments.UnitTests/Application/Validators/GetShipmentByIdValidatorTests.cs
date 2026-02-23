using Shipments.Application.UseCases.GetShipmentById;
using Shipments.Application.Validators;
using Xunit;

namespace Shipments.UnitTests.Application.Validators;

/// <summary>
/// Unit tests for GetShipmentByIdValidator.
/// </summary>
public class GetShipmentByIdValidatorTests
{
    [Fact]
    public void Validate_WithValidUuid_ReturnsNull()
    {
        // Arrange
        var validUuid = "550e8400-e29b-41d4-a716-446655440000";
        var input = new GetShipmentByIdInput { ShipmentId = validUuid };

        // Act
        var result = GetShipmentByIdValidator.Validate(input);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Validate_WithEmptyShipmentId_ReturnsError()
    {
        // Arrange
        var input = new GetShipmentByIdInput { ShipmentId = string.Empty };

        // Act
        var result = GetShipmentByIdValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("EMPTY_SHIPMENT_ID", result.Code);
        Assert.Equal(400, result.CorrespondingStatusCode);
    }

    [Fact]
    public void Validate_WithNullShipmentId_ReturnsError()
    {
        // Arrange
        var input = new GetShipmentByIdInput { ShipmentId = null };

        // Act
        var result = GetShipmentByIdValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("EMPTY_SHIPMENT_ID", result.Code);
        Assert.Equal(400, result.CorrespondingStatusCode);
    }

    [Fact]
    public void Validate_WithWhitespaceShipmentId_ReturnsError()
    {
        // Arrange
        var input = new GetShipmentByIdInput { ShipmentId = "   " };

        // Act
        var result = GetShipmentByIdValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("EMPTY_SHIPMENT_ID", result.Code);
        Assert.Equal(400, result.CorrespondingStatusCode);
    }

    [Fact]
    public void Validate_WithInvalidUuidFormat_ReturnsError()
    {
        // Arrange
        var input = new GetShipmentByIdInput { ShipmentId = "not-a-uuid" };

        // Act
        var result = GetShipmentByIdValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("INVALID_UUID_FORMAT", result.Code);
        Assert.Equal(400, result.CorrespondingStatusCode);
    }

    [Fact]
    public void Validate_WithPartialUuid_ReturnsError()
    {
        // Arrange
        var input = new GetShipmentByIdInput { ShipmentId = "550e8400-e29b-41d4-a716" };

        // Act
        var result = GetShipmentByIdValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("INVALID_UUID_FORMAT", result.Code);
        Assert.Equal(400, result.CorrespondingStatusCode);
    }

    [Fact]
    public void Validate_WithNumericString_ReturnsError()
    {
        // Arrange
        var input = new GetShipmentByIdInput { ShipmentId = "123456789" };

        // Act
        var result = GetShipmentByIdValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("INVALID_UUID_FORMAT", result.Code);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("550e8400-e29b-41d4-a716-446655440000")]
    [InlineData("6BA7B810-9DAD-11D1-80B4-00C04FD430C8")]
    public void Validate_WithVariousValidUuids_ReturnsNull(string uuid)
    {
        // Arrange
        var input = new GetShipmentByIdInput { ShipmentId = uuid };

        // Act
        var result = GetShipmentByIdValidator.Validate(input);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Validate_ErrorContainsCorrectMessage()
    {
        // Arrange
        var input = new GetShipmentByIdInput { ShipmentId = "invalid" };

        // Act
        var result = GetShipmentByIdValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Message);
        Assert.Contains("UUID", result.Message);
    }
}
