using System;
using Xunit;
using Shipments.Application.Validators;
using Shipments.Application.UseCases.CreateShipment;
using Shipments.Domain.Models;

namespace Shipments.UnitTests.Application.Validators;

/// <summary>
/// Unit tests for the CreateShipmentValidator class.
/// </summary>
public class CreateShipmentValidatorTests
{
    /// <summary>
    /// Valid shipment data should return null (no error).
    /// </summary>
    [Fact]
    public void Validate_WithValidData_ShouldReturnNull()
    {
        // Arrange
        var input = new CreateShipmentInput
        {
            PackageName = "Test Package",
            Weight = 10.5m,
            Dimensions = new Dimensions { Length = 10, Width = 20, Height = 30 },
            ShippingCost = 50.00m,
            DestinationAddress = "123 Main St, City, Country",
            Creator = "TestUser"
        };

        // Act
        var result = CreateShipmentValidator.Validate(input);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Empty PackageName should return error.
    /// </summary>
    [Fact]
    public void Validate_WithEmptyPackageName_ShouldReturnError()
    {
        // Arrange
        var input = new CreateShipmentInput
        {
            PackageName = string.Empty,
            Weight = 10.5m,
            Dimensions = new Dimensions { Length = 10, Width = 20, Height = 30 },
            ShippingCost = 50.00m,
            DestinationAddress = "123 Main St, City, Country",
            Creator = "TestUser"
        };

        // Act
        var result = CreateShipmentValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("EMPTY_PACKAGE_NAME", result!.Code);
        Assert.Equal(400, result.CorrespondingStatusCode);
    }

    /// <summary>
    /// Null PackageName should return error.
    /// </summary>
    [Fact]
    public void Validate_WithNullPackageName_ShouldReturnError()
    {
        // Arrange
        var input = new CreateShipmentInput
        {
            PackageName = null,
            Weight = 10.5m,
            Dimensions = new Dimensions { Length = 10, Width = 20, Height = 30 },
            ShippingCost = 50.00m,
            DestinationAddress = "123 Main St, City, Country",
            Creator = "TestUser"
        };

        // Act
        var result = CreateShipmentValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("EMPTY_PACKAGE_NAME", result!.Code);
    }

    /// <summary>
    /// PackageName with special characters should return error.
    /// </summary>
    [Fact]
    public void Validate_WithSpecialCharactersInPackageName_ShouldReturnError()
    {
        // Arrange
        var input = new CreateShipmentInput
        {
            PackageName = "Test@Package#123!",
            Weight = 10.5m,
            Dimensions = new Dimensions { Length = 10, Width = 20, Height = 30 },
            ShippingCost = 50.00m,
            DestinationAddress = "123 Main St, City, Country",
            Creator = "TestUser"
        };

        // Act
        var result = CreateShipmentValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("INVALID_PACKAGE_NAME", result!.Code);
    }

    /// <summary>
    /// Weight less than or equal to zero should return error.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-5.5)]
    public void Validate_WithInvalidWeight_ShouldReturnError(decimal weight)
    {
        // Arrange
        var input = new CreateShipmentInput
        {
            PackageName = "Test Package",
            Weight = weight,
            Dimensions = new Dimensions { Length = 10, Width = 20, Height = 30 },
            ShippingCost = 50.00m,
            DestinationAddress = "123 Main St, City, Country",
            Creator = "TestUser"
        };

        // Act
        var result = CreateShipmentValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("INVALID_WEIGHT", result!.Code);
        Assert.Equal(400, result.CorrespondingStatusCode);
    }

    /// <summary>
    /// Missing Dimensions should return error.
    /// </summary>
    [Fact]
    public void Validate_WithNullDimensions_ShouldReturnError()
    {
        // Arrange
        var input = new CreateShipmentInput
        {
            PackageName = "Test Package",
            Weight = 10.5m,
            Dimensions = null,
            ShippingCost = 50.00m,
            DestinationAddress = "123 Main St, City, Country",
            Creator = "TestUser"
        };

        // Act
        var result = CreateShipmentValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("MISSING_DIMENSIONS", result!.Code);
    }

    /// <summary>
    /// Dimensions with invalid values should return error.
    /// </summary>
    [Theory]
    [InlineData(0, 20, 30)]
    [InlineData(10, 0, 30)]
    [InlineData(10, 20, 0)]
    [InlineData(-5, 20, 30)]
    public void Validate_WithInvalidDimensions_ShouldReturnError(decimal length, decimal width, decimal height)
    {
        // Arrange
        var input = new CreateShipmentInput
        {
            PackageName = "Test Package",
            Weight = 10.5m,
            Dimensions = new Dimensions { Length = length, Width = width, Height = height },
            ShippingCost = 50.00m,
            DestinationAddress = "123 Main St, City, Country",
            Creator = "TestUser"
        };

        // Act
        var result = CreateShipmentValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("INVALID_DIMENSIONS", result!.Code);
    }

    /// <summary>
    /// ShippingCost less than or equal to zero should return error.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-25.50)]
    public void Validate_WithInvalidShippingCost_ShouldReturnError(decimal shippingCost)
    {
        // Arrange
        var input = new CreateShipmentInput
        {
            PackageName = "Test Package",
            Weight = 10.5m,
            Dimensions = new Dimensions { Length = 10, Width = 20, Height = 30 },
            ShippingCost = shippingCost,
            DestinationAddress = "123 Main St, City, Country",
            Creator = "TestUser"
        };

        // Act
        var result = CreateShipmentValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("INVALID_SHIPPING_COST", result!.Code);
    }

    /// <summary>
    /// Empty DestinationAddress should return error.
    /// </summary>
    [Fact]
    public void Validate_WithEmptyDestinationAddress_ShouldReturnError()
    {
        // Arrange
        var input = new CreateShipmentInput
        {
            PackageName = "Test Package",
            Weight = 10.5m,
            Dimensions = new Dimensions { Length = 10, Width = 20, Height = 30 },
            ShippingCost = 50.00m,
            DestinationAddress = string.Empty,
            Creator = "TestUser"
        };

        // Act
        var result = CreateShipmentValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("EMPTY_DESTINATION_ADDRESS", result!.Code);
    }

    /// <summary>
    /// Null DestinationAddress should return error.
    /// </summary>
    [Fact]
    public void Validate_WithNullDestinationAddress_ShouldReturnError()
    {
        // Arrange
        var input = new CreateShipmentInput
        {
            PackageName = "Test Package",
            Weight = 10.5m,
            Dimensions = new Dimensions { Length = 10, Width = 20, Height = 30 },
            ShippingCost = 50.00m,
            DestinationAddress = null,
            Creator = "TestUser"
        };

        // Act
        var result = CreateShipmentValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("EMPTY_DESTINATION_ADDRESS", result!.Code);
    }

    /// <summary>
    /// Empty Creator should return error.
    /// </summary>
    [Fact]
    public void Validate_WithEmptyCreator_ShouldReturnError()
    {
        // Arrange
        var input = new CreateShipmentInput
        {
            PackageName = "Test Package",
            Weight = 10.5m,
            Dimensions = new Dimensions { Length = 10, Width = 20, Height = 30 },
            ShippingCost = 50.00m,
            DestinationAddress = "123 Main St, City, Country",
            Creator = string.Empty
        };

        // Act
        var result = CreateShipmentValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("EMPTY_CREATOR", result!.Code);
    }

    /// <summary>
    /// Null Creator should return error.
    /// </summary>
    [Fact]
    public void Validate_WithNullCreator_ShouldReturnError()
    {
        // Arrange
        var input = new CreateShipmentInput
        {
            PackageName = "Test Package",
            Weight = 10.5m,
            Dimensions = new Dimensions { Length = 10, Width = 20, Height = 30 },
            ShippingCost = 50.00m,
            DestinationAddress = "123 Main St, City, Country",
            Creator = null
        };

        // Act
        var result = CreateShipmentValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("EMPTY_CREATOR", result!.Code);
    }
}
