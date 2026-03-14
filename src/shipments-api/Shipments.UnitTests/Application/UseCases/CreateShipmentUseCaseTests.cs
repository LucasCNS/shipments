using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Shipments.Application.Repositories;
using Shipments.Application.Results;
using Shipments.Application.UseCases.CreateShipment;
using Shipments.Domain.Models;

namespace Shipments.UnitTests.Application.UseCases;

/// <summary>
/// Unit tests for the CreateShipmentUseCase class.
/// </summary>
public class CreateShipmentUseCaseTests
{
    private readonly Mock<IShipmentRepository> _repositoryMock;
    private readonly Mock<ILogger<CreateShipmentUseCase>> _loggerMock;
    private readonly CreateShipmentUseCase _useCase;

    /// <summary>
    /// Initializes test fixtures.
    /// </summary>
    public CreateShipmentUseCaseTests()
    {
        _repositoryMock = new Mock<IShipmentRepository>();
        _loggerMock = new Mock<ILogger<CreateShipmentUseCase>>();
        _useCase = new CreateShipmentUseCase(_repositoryMock.Object, _loggerMock.Object);
    }

    /// <summary>
    /// Valid input should return CreateShipmentOutput with generated UUID.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithValidInput_ShouldReturnOutputWithUuid()
    {
        // Arrange
        var input = new CreateShipmentInput
        {
            PackageName = "Valid Package",
            Weight = 15.5m,
            Dimensions = new Dimensions { Length = 25, Width = 35, Height = 45 },
            ShippingCost = 100.00m,
            DestinationAddress = "456 Oak Ave, Town, Country",
            Creator = "Alice"
        };

        var capturedShipment = (Shipment?)null;
        _repositoryMock
            .Setup(repo => repo.CreateAsync(It.IsAny<Shipment>()))
            .Callback<Shipment>(s => capturedShipment = s)
            .ReturnsAsync((Shipment s) => s);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess, "Result should be successful");
        Assert.NotNull(result.Value);
        Assert.NotEqual(Guid.Empty, result.Value.Id); // ID should be generated, not empty
        Assert.Equal("Valid Package", result.Value.PackageName);
        Assert.Equal(15.5m, result.Value.Weight);
        Assert.NotNull(result.Value.Dimensions);
        Assert.Equal(25, result.Value.Dimensions.Length);
        Assert.Equal(35, result.Value.Dimensions.Width);
        Assert.Equal(45, result.Value.Dimensions.Height);
        Assert.Equal(100.00m, result.Value.ShippingCost);
        Assert.Equal("456 Oak Ave, Town, Country", result.Value.DestinationAddress);
        Assert.Equal("Alice", result.Value.Creator);
        Assert.Equal("pending", result.Value.Status);
        _repositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Shipment>()), Times.Once);
    }

    /// <summary>
    /// Valid input should set DateCreated and DateLastUpdated to UTC now.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithValidInput_ShouldSetDatesCorrectly()
    {
        // Arrange
        var beforeCall = DateTime.UtcNow;
        var input = new CreateShipmentInput
        {
            PackageName = "Date Test Package",
            Weight = 5.0m,
            Dimensions = new Dimensions { Length = 10, Width = 10, Height = 10 },
            ShippingCost = 25.00m,
            DestinationAddress = "789 Elm St, Village, Country",
            Creator = "Bob"
        };

        _repositoryMock
            .Setup(repo => repo.CreateAsync(It.IsAny<Shipment>()))
            .ReturnsAsync((Shipment s) => s);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);
        var afterCall = DateTime.UtcNow;

        // Assert
        Assert.True(result.IsSuccess, "Result should be successful");
        Assert.NotNull(result.Value);
        Assert.True(result.Value.DateCreated >= beforeCall && result.Value.DateCreated <= afterCall);
        Assert.True(result.Value.DateLastUpdated >= beforeCall && result.Value.DateLastUpdated <= afterCall);
    }

    /// <summary>
    /// Empty PackageName should return output with validation error.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithEmptyPackageName_ShouldReturnError()
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
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess, "Result should fail for empty PackageName");
        Assert.NotNull(result.Error);
        Assert.Equal("VALIDATION_ERROR", result.Error!.Code);
        Assert.Equal(400, result.Error.CorrespondingStatusCode);
        Assert.True(result.Error.ValidationErrors.Count > 0, "Error should contain validation messages");
        _repositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Shipment>()), Times.Never);
    }

    /// <summary>
    /// Invalid Weight should return output with validation error.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithInvalidWeight_ShouldReturnError()
    {
        // Arrange
        var input = new CreateShipmentInput
        {
            PackageName = "Test Package",
            Weight = -5.0m,
            Dimensions = new Dimensions { Length = 10, Width = 20, Height = 30 },
            ShippingCost = 50.00m,
            DestinationAddress = "123 Main St, City, Country",
            Creator = "TestUser"
        };

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess, "Result should fail for invalid Weight");
        Assert.NotNull(result.Error);
        Assert.Equal("VALIDATION_ERROR", result.Error!.Code);
        _repositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Shipment>()), Times.Never);
    }

    /// <summary>
    /// Invalid Dimensions should return output with validation error.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithInvalidDimensions_ShouldReturnError()
    {
        // Arrange
        var input = new CreateShipmentInput
        {
            PackageName = "Test Package",
            Weight = 10.5m,
            Dimensions = new Dimensions { Length = 0, Width = 20, Height = 30 },
            ShippingCost = 50.00m,
            DestinationAddress = "123 Main St, City, Country",
            Creator = "TestUser"
        };

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess, "Result should fail for invalid Dimensions");
        Assert.NotNull(result.Error);
        Assert.Equal("VALIDATION_ERROR", result.Error!.Code);
        _repositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Shipment>()), Times.Never);
    }

    /// <summary>
    /// Invalid ShippingCost should return output with validation error.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithInvalidShippingCost_ShouldReturnError()
    {
        // Arrange
        var input = new CreateShipmentInput
        {
            PackageName = "Test Package",
            Weight = 10.5m,
            Dimensions = new Dimensions { Length = 10, Width = 20, Height = 30 },
            ShippingCost = -10.00m,
            DestinationAddress = "123 Main St, City, Country",
            Creator = "TestUser"
        };

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess, "Result should fail for invalid ShippingCost");
        Assert.NotNull(result.Error);
        Assert.Equal("VALIDATION_ERROR", result.Error!.Code);
        _repositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Shipment>()), Times.Never);
    }

    /// <summary>
    /// Empty DestinationAddress should return output with validation error.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithEmptyDestinationAddress_ShouldReturnError()
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
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess, "Result should fail for empty DestinationAddress");
        Assert.NotNull(result.Error);
        Assert.Equal("VALIDATION_ERROR", result.Error!.Code);
        _repositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Shipment>()), Times.Never);
    }

    /// <summary>
    /// Empty Creator should return output with validation error.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithEmptyCreator_ShouldReturnError()
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
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess, "Result should fail for empty Creator");
        Assert.NotNull(result.Error);
        Assert.Equal("VALIDATION_ERROR", result.Error!.Code);
        _repositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Shipment>()), Times.Never);
    }

    /// <summary>
    /// Valid input should call repository exactly once.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithValidInput_ShouldCallRepositoryOnce()
    {
        // Arrange
        var input = new CreateShipmentInput
        {
            PackageName = "Repository Test",
            Weight = 8.5m,
            Dimensions = new Dimensions { Length = 15, Width = 25, Height = 35 },
            ShippingCost = 75.00m,
            DestinationAddress = "321 Pine Rd, City, Country",
            Creator = "Charlie"
        };

        _repositoryMock
            .Setup(repo => repo.CreateAsync(It.IsAny<Shipment>()))
            .ReturnsAsync((Shipment s) => s);

        // Act
        await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Shipment>()), Times.Once);
    }

    /// <summary>
    /// Special characters in PackageName should return error.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithSpecialCharactersInPackageName_ShouldReturnError()
    {
        // Arrange
        var input = new CreateShipmentInput
        {
            PackageName = "Invalid@Package#",
            Weight = 10.5m,
            Dimensions = new Dimensions { Length = 10, Width = 20, Height = 30 },
            ShippingCost = 50.00m,
            DestinationAddress = "123 Main St, City, Country",
            Creator = "TestUser"
        };

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess, "Result should fail for invalid PackageName");
        Assert.NotNull(result.Error);
        Assert.Equal("VALIDATION_ERROR", result.Error!.Code);
    }
}
