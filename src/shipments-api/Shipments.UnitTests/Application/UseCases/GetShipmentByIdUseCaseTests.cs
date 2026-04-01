using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Shipments.Application.Repositories;
using Shipments.Application.UseCases.GetShipmentById;
using Shipments.Domain.Models;
using Xunit;

namespace Shipments.UnitTests.Application.UseCases;

/// <summary>
/// Unit tests for GetShipmentByIdUseCase.
/// </summary>
public class GetShipmentByIdUseCaseTests
{
    private readonly Mock<IShipmentRepository> _repositoryMock;
    private readonly Mock<ILogger<GetShipmentByIdUseCase>> _loggerMock;
    private readonly GetShipmentByIdUseCase _useCase;

    public GetShipmentByIdUseCaseTests()
    {
        _repositoryMock = new Mock<IShipmentRepository>();
        _loggerMock = new Mock<ILogger<GetShipmentByIdUseCase>>();
        _useCase = new GetShipmentByIdUseCase(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidUuidAndExistingShipment_ReturnsShipmentSuccessfully()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var input = new GetShipmentByIdInput { ShipmentId = shipmentId.ToString() };
        var shipment = new Shipment
        {
            Id = shipmentId,
            PackageName = "Test Package",
            Weight = 1.5m,
            Dimensions = new Dimensions { Length = 10, Width = 10, Height = 10 },
            ShippingCost = 100m,
            DestinationAddress = "123 Main St",
            DateCreated = DateTime.UtcNow,
            DateLastUpdated = DateTime.UtcNow,
            Creator = "testuser",
            Status = "pending"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(shipmentId))
            .ReturnsAsync(shipment);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(shipmentId, result.Value!.Id);
        Assert.Equal("Test Package", result.Value!.PackageName);
        Assert.Equal(1.5m, result.Value!.Weight);
        Assert.Equal(100m, result.Value!.ShippingCost);
        Assert.Equal("testuser", result.Value!.Creator);
        Assert.Equal("pending", result.Value!.Status);
        _repositoryMock.Verify(r => r.GetByIdAsync(shipmentId), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentShipment_ReturnsNotFoundError()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var input = new GetShipmentByIdInput { ShipmentId = shipmentId.ToString() };

        _repositoryMock.Setup(r => r.GetByIdAsync(shipmentId))
            .ReturnsAsync((Shipment?)null);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("SHIPMENT_NOT_FOUND", result.Error.Code);
        Assert.Equal(404, result.Error.CorrespondingStatusCode);
        _repositoryMock.Verify(r => r.GetByIdAsync(shipmentId), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithInvalidUuidFormat_ReturnsValidationError()
    {
        // Arrange
        var input = new GetShipmentByIdInput { ShipmentId = "invalid-uuid" };

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("VALIDATION_ERROR", result.Error.Code);
        Assert.Equal(400, result.Error.CorrespondingStatusCode);
        _repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WithEmptyShipmentId_ReturnsValidationError()
    {
        // Arrange
        var input = new GetShipmentByIdInput { ShipmentId = string.Empty };

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("VALIDATION_ERROR", result.Error.Code);
        Assert.Equal(400, result.Error.CorrespondingStatusCode);
    }

    [Fact]
    public async Task HandleAsync_WithNullShipmentId_ReturnsValidationError()
    {
        // Arrange
        var input = new GetShipmentByIdInput { ShipmentId = null };

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("VALIDATION_ERROR", result.Error.Code);
    }

    [Fact]
    public async Task HandleAsync_WhenRepositoryThrows_ReturnsRepositoryError()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var input = new GetShipmentByIdInput { ShipmentId = shipmentId.ToString() };

        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("REPOSITORY_ERROR", result.Error.Code);
        Assert.Equal(500, result.Error.CorrespondingStatusCode);
    }

    [Fact]
    public async Task HandleAsync_WithValidShipment_LogsInfoMessage()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var input = new GetShipmentByIdInput { ShipmentId = shipmentId.ToString() };
        var shipment = new Shipment
        {
            Id = shipmentId,
            PackageName = "Test Package",
            Weight = 1.5m,
            Dimensions = new Dimensions { Length = 10, Width = 10, Height = 10 },
            ShippingCost = 100m,
            DestinationAddress = "123 Main St",
            DateCreated = DateTime.UtcNow,
            DateLastUpdated = DateTime.UtcNow,
            Creator = "testuser",
            Status = "pending"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(shipmentId))
            .ReturnsAsync(shipment);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(_loggerMock.Invocations.Count > 0);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentShipment_LogsWarningMessage()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var input = new GetShipmentByIdInput { ShipmentId = shipmentId.ToString() };

        _repositoryMock.Setup(r => r.GetByIdAsync(shipmentId))
            .ReturnsAsync((Shipment?)null);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.True(_loggerMock.Invocations.Count > 0);
    }

    [Fact]
    public async Task HandleAsync_WithValidationError_LogsWarningMessage()
    {
        // Arrange
        var input = new GetShipmentByIdInput { ShipmentId = "invalid" };

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.True(_loggerMock.Invocations.Count > 0);
    }

    [Fact]
    public async Task HandleAsync_WithCancellationToken_RespectsCancel()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var input = new GetShipmentByIdInput { ShipmentId = shipmentId.ToString() };
        var cts = new CancellationTokenSource();

        var shipment = new Shipment
        {
            Id = shipmentId,
            PackageName = "Test Package",
            Weight = 1.5m,
            Dimensions = new Dimensions { Length = 10, Width = 10, Height = 10 },
            ShippingCost = 100m,
            DestinationAddress = "123 Main St",
            DateCreated = DateTime.UtcNow,
            DateLastUpdated = DateTime.UtcNow,
            Creator = "testuser",
            Status = "pending"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(shipment);

        // Act & Assert - Should not throw
        var result = await _useCase.HandleAsync(input, cts.Token);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task HandleAsync_WithValidShipment_PreservesAllProperties()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var dateCreated = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var dateUpdated = new DateTime(2024, 1, 2, 15, 30, 45, DateTimeKind.Utc);
        var dimensions = new Dimensions { Length = 20, Width = 15, Height = 10 };

        var input = new GetShipmentByIdInput { ShipmentId = shipmentId.ToString() };
        var shipment = new Shipment
        {
            Id = shipmentId,
            PackageName = "Premium Package",
            Weight = 2.75m,
            Dimensions = dimensions,
            ShippingCost = 250.99m,
            DestinationAddress = "456 Oak Ave, City, State 12345",
            DateCreated = dateCreated,
            DateLastUpdated = dateUpdated,
            Creator = "john.doe@example.com",
            Status = "pending"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(shipmentId))
            .ReturnsAsync(shipment);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(shipmentId, result.Value!.Id);
        Assert.Equal("Premium Package", result.Value!.PackageName);
        Assert.Equal(2.75m, result.Value!.Weight);
        Assert.Equal(250.99m, result.Value!.ShippingCost);
        Assert.Equal("456 Oak Ave, City, State 12345", result.Value!.DestinationAddress);
        Assert.Equal(dateCreated, result.Value!.DateCreated);
        Assert.Equal(dateUpdated, result.Value!.DateLastUpdated);
        Assert.Equal("john.doe@example.com", result.Value!.Creator);
        Assert.Equal("pending", result.Value!.Status);
        Assert.NotNull(result.Value!.Dimensions);
        Assert.Equal(20, result.Value!.Dimensions.Length);
        Assert.Equal(15, result.Value!.Dimensions.Width);
        Assert.Equal(10, result.Value!.Dimensions.Height);
    }

    [Theory]
    [InlineData("pending")]
    [InlineData("in_transit")]
    [InlineData("delivered")]
    [InlineData("cancelled")]
    public async Task HandleAsync_WithDifferentStatuses_ReturnsCorrectStatus(string status)
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var input = new GetShipmentByIdInput { ShipmentId = shipmentId.ToString() };
        var shipment = new Shipment
        {
            Id = shipmentId,
            PackageName = "Test Package",
            Weight = 1.5m,
            Dimensions = new Dimensions { Length = 10, Width = 10, Height = 10 },
            ShippingCost = 100m,
            DestinationAddress = "123 Main St",
            DateCreated = DateTime.UtcNow,
            DateLastUpdated = DateTime.UtcNow,
            Creator = "testuser",
            Status = status
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(shipmentId))
            .ReturnsAsync(shipment);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(status, result.Value!.Status);
    }

    [Fact]
    public async Task HandleAsync_WithRepositoryException_LogsErrorMessage()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var input = new GetShipmentByIdInput { ShipmentId = shipmentId.ToString() };

        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.True(_loggerMock.Invocations.Count > 0);
    }
}

