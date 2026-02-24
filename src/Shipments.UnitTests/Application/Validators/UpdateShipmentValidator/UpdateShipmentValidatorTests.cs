using System;
using Xunit;
using Shipments.Application.UseCases.UpdateShipment;
using Shipments.Application.Validators;
using Shipments.Domain.Models;

namespace Shipments.UnitTests.Application.Validators;

/// <summary>
/// Unit tests for the UpdateShipmentValidator class.
/// </summary>
public class UpdateShipmentValidatorTests
{
    /// <summary>
    /// Validator should return null when all required fields are valid.
    /// </summary>
    [Fact]
    public void Validate_WithValidInput_ShouldReturnNull()
    {
        // Arrange
        var input = new UpdateShipmentInput
        {
            Id = Guid.NewGuid().ToString(),
            Creator = "TestUser",
            PackageName = "Updated Package"
        };

        // Act
        var result = UpdateShipmentValidator.Validate(input);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Validator should return error when ID is empty.
    /// </summary>
    [Fact]
    public void Validate_WithEmptyId_ShouldReturnError()
    {
        // Arrange
        var input = new UpdateShipmentInput
        {
            Id = string.Empty,
            Creator = "TestUser"
        };

        // Act
        var result = UpdateShipmentValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("INVALID_SHIPMENT_ID", result.Code);
        Assert.Equal(400, result.CorrespondingStatusCode);
    }

    /// <summary>
    /// Validator should return error when ID is not a valid GUID.
    /// </summary>
    [Fact]
    public void Validate_WithInvalidGuidId_ShouldReturnError()
    {
        // Arrange
        var input = new UpdateShipmentInput
        {
            Id = "not-a-valid-guid",
            Creator = "TestUser"
        };

        // Act
        var result = UpdateShipmentValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("INVALID_SHIPMENT_ID", result.Code);
        Assert.Equal(400, result.CorrespondingStatusCode);
    }

    /// <summary>
    /// Validator should return error when Creator is empty.
    /// </summary>
    [Fact]
    public void Validate_WithEmptyCreator_ShouldReturnError()
    {
        // Arrange
        var input = new UpdateShipmentInput
        {
            Id = Guid.NewGuid().ToString(),
            Creator = string.Empty
        };

        // Act
        var result = UpdateShipmentValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("EMPTY_CREATOR", result.Code);
        Assert.Equal(400, result.CorrespondingStatusCode);
    }

    /// <summary>
    /// Validator should return error when PackageName contains special characters.
    /// </summary>
    [Fact]
    public void Validate_WithSpecialCharactersInPackageName_ShouldReturnError()
    {
        // Arrange
        var input = new UpdateShipmentInput
        {
            Id = Guid.NewGuid().ToString(),
            Creator = "TestUser",
            PackageName = "Invalid@Package#Name"
        };

        // Act
        var result = UpdateShipmentValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("INVALID_PACKAGE_NAME", result.Code);
        Assert.Equal(400, result.CorrespondingStatusCode);
    }

    /// <summary>
    /// Validator should return error when Weight is less than or equal to zero.
    /// </summary>
    [Fact]
    public void Validate_WithInvalidWeight_ShouldReturnError()
    {
        // Arrange
        var input = new UpdateShipmentInput
        {
            Id = Guid.NewGuid().ToString(),
            Creator = "TestUser",
            Weight = 0
        };

        // Act
        var result = UpdateShipmentValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("INVALID_WEIGHT", result.Code);
        Assert.Equal(400, result.CorrespondingStatusCode);
    }

    /// <summary>
    /// Validator should return error when Dimensions have invalid values.
    /// </summary>
    [Fact]
    public void Validate_WithInvalidDimensions_ShouldReturnError()
    {
        // Arrange
        var input = new UpdateShipmentInput
        {
            Id = Guid.NewGuid().ToString(),
            Creator = "TestUser",
            Dimensions = new Dimensions { Length = 0, Width = 10, Height = 10 }
        };

        // Act
        var result = UpdateShipmentValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("INVALID_DIMENSIONS", result.Code);
        Assert.Equal(400, result.CorrespondingStatusCode);
    }

    /// <summary>
    /// Validator should return error when ShippingCost is less than or equal to zero.
    /// </summary>
    [Fact]
    public void Validate_WithInvalidShippingCost_ShouldReturnError()
    {
        // Arrange
        var input = new UpdateShipmentInput
        {
            Id = Guid.NewGuid().ToString(),
            Creator = "TestUser",
            ShippingCost = -10m
        };

        // Act
        var result = UpdateShipmentValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("INVALID_SHIPPING_COST", result.Code);
        Assert.Equal(400, result.CorrespondingStatusCode);
    }

    /// <summary>
    /// Validator should allow null optional fields.
    /// </summary>
    [Fact]
    public void Validate_WithNullOptionalFields_ShouldReturnNull()
    {
        // Arrange
        var input = new UpdateShipmentInput
        {
            Id = Guid.NewGuid().ToString(),
            Creator = "TestUser",
            PackageName = null,
            Weight = null,
            Dimensions = null,
            ShippingCost = null,
            DestinationAddress = null
        };

        // Act
        var result = UpdateShipmentValidator.Validate(input);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Validator should allow valid PackageName with alphanumeric characters.
    /// </summary>
    [Fact]
    public void Validate_WithValidPackageName_ShouldReturnNull()
    {
        // Arrange
        var input = new UpdateShipmentInput
        {
            Id = Guid.NewGuid().ToString(),
            Creator = "TestUser",
            PackageName = "Package 123 with-hyphen_underscore"
        };

        // Act
        var result = UpdateShipmentValidator.Validate(input);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Validator should allow valid Weight greater than zero.
    /// </summary>
    [Fact]
    public void Validate_WithValidWeight_ShouldReturnNull()
    {
        // Arrange
        var input = new UpdateShipmentInput
        {
            Id = Guid.NewGuid().ToString(),
            Creator = "TestUser",
            Weight = 10.5m
        };

        // Act
        var result = UpdateShipmentValidator.Validate(input);

        // Assert
        Assert.Null(result);
    }
}
