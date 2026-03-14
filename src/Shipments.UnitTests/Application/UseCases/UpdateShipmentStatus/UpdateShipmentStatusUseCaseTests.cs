using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Shipments.Application.Repositories;
using Shipments.Application.Results;
using Shipments.Application.UseCases.UpdateShipmentStatus;
using Shipments.Domain.Models;

namespace Shipments.UnitTests.Application.UseCases.UpdateShipmentStatus;

/// <summary>
/// Unit tests for the UpdateShipmentStatusUseCase class.
/// </summary>
public class UpdateShipmentStatusUseCaseTests
{
    private readonly Mock<IShipmentRepository> _repositoryMock;
    private readonly Mock<ILogger<UpdateShipmentStatusUseCase>> _loggerMock;
    private readonly UpdateShipmentStatusUseCase _useCase;

    /// <summary>
    /// Initializes test fixtures.
    /// </summary>
    public UpdateShipmentStatusUseCaseTests()
    {
        _repositoryMock = new Mock<IShipmentRepository>();
        _loggerMock = new Mock<ILogger<UpdateShipmentStatusUseCase>>();
        _useCase = new UpdateShipmentStatusUseCase(_repositoryMock.Object, _loggerMock.Object);
    }

    // ==================== Valid Transitions ====================

    /// <summary>
    /// Test: Valid transition from pending to in_transit should return 200 with updated status.
    /// </summary>
    [Fact]
    public async Task HandleAsync_PendingToInTransit_ShouldReturnSuccess()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var shipment = new Shipment
        {
            Id = shipmentId,
            PackageName = "Test Package",
            Weight = 10m,
            Dimensions = new Dimensions { Length = 10, Width = 20, Height = 30 },
            ShippingCost = 50m,
            DestinationAddress = "123 Main St",
            DateCreated = DateTime.UtcNow,
            DateLastUpdated = DateTime.UtcNow,
            Creator = "TestUser",
            Status = "pending"
        };

        var input = new UpdateShipmentStatusInput
        {
            Id = shipmentId.ToString(),
            Status = "in_transit",
            Creator = "TestUser"
        };

        var beforeUpdate = DateTime.UtcNow;
        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(shipmentId))
            .ReturnsAsync(shipment);

        _repositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Shipment>()))
            .ReturnsAsync((Shipment s) => s);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);
        var afterUpdate = DateTime.UtcNow;

        // Assert
        Assert.True(result.IsSuccess, "Result should be successful");
        Assert.NotNull(result.Value);
        Assert.Equal("in_transit", result.Value.Status);
        Assert.True(result.Value.DateLastUpdated >= beforeUpdate && result.Value.DateLastUpdated <= afterUpdate);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Shipment>()), Times.Once);
    }

    /// <summary>
    /// Test: Valid transition from pending to cancelled should return 200 with updated status.
    /// </summary>
    [Fact]
    public async Task HandleAsync_PendingToCancelled_ShouldReturnSuccess()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var shipment = new Shipment
        {
            Id = shipmentId,
            PackageName = "Test Package",
            Weight = 10m,
            Dimensions = new Dimensions { Length = 10, Width = 20, Height = 30 },
            ShippingCost = 50m,
            DestinationAddress = "123 Main St",
            DateCreated = DateTime.UtcNow,
            DateLastUpdated = DateTime.UtcNow,
            Creator = "TestUser",
            Status = "pending"
        };

        var input = new UpdateShipmentStatusInput
        {
            Id = shipmentId.ToString(),
            Status = "cancelled",
            Creator = "TestUser"
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(shipmentId))
            .ReturnsAsync(shipment);

        _repositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Shipment>()))
            .ReturnsAsync((Shipment s) => s);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess, "Result should be successful");
        Assert.NotNull(result.Value);
        Assert.Equal("cancelled", result.Value.Status);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Shipment>()), Times.Once);
    }

    /// <summary>
    /// Test: Valid transition from in_transit to delivered should return 200 with updated status.
    /// </summary>
    [Fact]
    public async Task HandleAsync_InTransitToDelivered_ShouldReturnSuccess()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var shipment = new Shipment
        {
            Id = shipmentId,
            PackageName = "Test Package",
            Weight = 10m,
            Dimensions = new Dimensions { Length = 10, Width = 20, Height = 30 },
            ShippingCost = 50m,
            DestinationAddress = "123 Main St",
            DateCreated = DateTime.UtcNow,
            DateLastUpdated = DateTime.UtcNow,
            Creator = "TestUser",
            Status = "in_transit"
        };

        var input = new UpdateShipmentStatusInput
        {
            Id = shipmentId.ToString(),
            Status = "delivered",
            Creator = "TestUser"
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(shipmentId))
            .ReturnsAsync(shipment);

        _repositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Shipment>()))
            .ReturnsAsync((Shipment s) => s);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess, "Result should be successful");
        Assert.NotNull(result.Value);
        Assert.Equal("delivered", result.Value.Status);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Shipment>()), Times.Once);
    }

    /// <summary>
    /// Test: Valid transition from in_transit to cancelled should return 200 with updated status.
    /// </summary>
    [Fact]
    public async Task HandleAsync_InTransitToCancelled_ShouldReturnSuccess()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var shipment = new Shipment
        {
            Id = shipmentId,
            PackageName = "Test Package",
            Weight = 10m,
            Dimensions = new Dimensions { Length = 10, Width = 20, Height = 30 },
            ShippingCost = 50m,
            DestinationAddress = "123 Main St",
            DateCreated = DateTime.UtcNow,
            DateLastUpdated = DateTime.UtcNow,
            Creator = "TestUser",
            Status = "in_transit"
        };

        var input = new UpdateShipmentStatusInput
        {
            Id = shipmentId.ToString(),
            Status = "cancelled",
            Creator = "TestUser"
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(shipmentId))
            .ReturnsAsync(shipment);

        _repositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Shipment>()))
            .ReturnsAsync((Shipment s) => s);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess, "Result should be successful");
        Assert.NotNull(result.Value);
        Assert.Equal("cancelled", result.Value.Status);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Shipment>()), Times.Once);
    }

    // ==================== Invalid Transitions ====================

    /// <summary>
    /// Test: Attempting to change status from delivered (final state) should return 409 Conflict.
    /// </summary>
    [Fact]
    public async Task HandleAsync_DeliveredToAny_ShouldReturnConflict()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var shipment = new Shipment
        {
            Id = shipmentId,
            PackageName = "Test Package",
            Weight = 10m,
            Dimensions = new Dimensions { Length = 10, Width = 20, Height = 30 },
            ShippingCost = 50m,
            DestinationAddress = "123 Main St",
            DateCreated = DateTime.UtcNow,
            DateLastUpdated = DateTime.UtcNow,
            Creator = "TestUser",
            Status = "delivered"
        };

        var input = new UpdateShipmentStatusInput
        {
            Id = shipmentId.ToString(),
            Status = "cancelled",
            Creator = "TestUser"
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(shipmentId))
            .ReturnsAsync(shipment);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess, "Result should fail for final state transition");
        Assert.NotNull(result.Error);
        Assert.Equal("INVALID_STATUS_TRANSITION", result.Error!.Code);
        Assert.Equal(409, result.Error.CorrespondingStatusCode);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Shipment>()), Times.Never);
    }

    /// <summary>
    /// Test: Attempting to change status from cancelled (final state) should return 409 Conflict.
    /// </summary>
    [Fact]
    public async Task HandleAsync_CancelledToAny_ShouldReturnConflict()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var shipment = new Shipment
        {
            Id = shipmentId,
            PackageName = "Test Package",
            Weight = 10m,
            Dimensions = new Dimensions { Length = 10, Width = 20, Height = 30 },
            ShippingCost = 50m,
            DestinationAddress = "123 Main St",
            DateCreated = DateTime.UtcNow,
            DateLastUpdated = DateTime.UtcNow,
            Creator = "TestUser",
            Status = "cancelled"
        };

        var input = new UpdateShipmentStatusInput
        {
            Id = shipmentId.ToString(),
            Status = "in_transit",
            Creator = "TestUser"
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(shipmentId))
            .ReturnsAsync(shipment);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess, "Result should fail for final state transition");
        Assert.NotNull(result.Error);
        Assert.Equal("INVALID_STATUS_TRANSITION", result.Error!.Code);
        Assert.Equal(409, result.Error.CorrespondingStatusCode);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Shipment>()), Times.Never);
    }

    /// <summary>
    /// Test: Attempting to change to the same status should return 409 Conflict.
    /// </summary>
    [Fact]
    public async Task HandleAsync_SameStatus_ShouldReturnConflict()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var shipment = new Shipment
        {
            Id = shipmentId,
            PackageName = "Test Package",
            Weight = 10m,
            Dimensions = new Dimensions { Length = 10, Width = 20, Height = 30 },
            ShippingCost = 50m,
            DestinationAddress = "123 Main St",
            DateCreated = DateTime.UtcNow,
            DateLastUpdated = DateTime.UtcNow,
            Creator = "TestUser",
            Status = "pending"
        };

        var input = new UpdateShipmentStatusInput
        {
            Id = shipmentId.ToString(),
            Status = "pending",
            Creator = "TestUser"
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(shipmentId))
            .ReturnsAsync(shipment);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess, "Result should fail for same status");
        Assert.NotNull(result.Error);
        Assert.Equal("INVALID_STATUS_TRANSITION", result.Error!.Code);
        Assert.Equal(409, result.Error.CorrespondingStatusCode);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Shipment>()), Times.Never);
    }

    /// <summary>
    /// Test: Shipment not found should return 404 Not Found.
    /// </summary>
    [Fact]
    public async Task HandleAsync_ShipmentNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var input = new UpdateShipmentStatusInput
        {
            Id = shipmentId.ToString(),
            Status = "in_transit",
            Creator = "TestUser"
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(shipmentId))
            .ReturnsAsync((Shipment?)null);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess, "Result should fail for non-existent shipment");
        Assert.NotNull(result.Error);
        Assert.Equal("SHIPMENT_NOT_FOUND", result.Error!.Code);
        Assert.Equal(404, result.Error.CorrespondingStatusCode);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Shipment>()), Times.Never);
    }

    /// <summary>
    /// Test: Invalid UUID format should return 400 Bad Request.
    /// </summary>
    [Fact]
    public async Task HandleAsync_InvalidUUID_ShouldReturnBadRequest()
    {
        // Arrange
        var input = new UpdateShipmentStatusInput
        {
            Id = "not-a-uuid",
            Status = "in_transit",
            Creator = "TestUser"
        };

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess, "Result should fail for invalid UUID");
        Assert.NotNull(result.Error);
        Assert.Equal("VALIDATION_ERROR", result.Error!.Code);
        Assert.Equal(400, result.Error.CorrespondingStatusCode);
        _repositoryMock.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    /// <summary>
    /// Test: Null/empty status should return 400 Bad Request.
    /// </summary>
    [Fact]
    public async Task HandleAsync_NullStatus_ShouldReturnBadRequest()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var input = new UpdateShipmentStatusInput
        {
            Id = shipmentId.ToString(),
            Status = null,
            Creator = "TestUser"
        };

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess, "Result should fail for null status");
        Assert.NotNull(result.Error);
        Assert.Equal("VALIDATION_ERROR", result.Error!.Code);
        Assert.Equal(400, result.Error.CorrespondingStatusCode);
        _repositoryMock.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    /// <summary>
    /// Test: Invalid status value should return 400 Bad Request.
    /// </summary>
    [Fact]
    public async Task HandleAsync_InvalidStatusValue_ShouldReturnBadRequest()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var input = new UpdateShipmentStatusInput
        {
            Id = shipmentId.ToString(),
            Status = "invalid_status",
            Creator = "TestUser"
        };

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess, "Result should fail for invalid status value");
        Assert.NotNull(result.Error);
        Assert.Equal("VALIDATION_ERROR", result.Error!.Code);
        Assert.Equal(400, result.Error.CorrespondingStatusCode);
        _repositoryMock.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    /// <summary>
    /// Test: Repository exception should return 500 Internal Server Error.
    /// </summary>
    [Fact]
    public async Task HandleAsync_RepositoryException_ShouldReturn500()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var shipment = new Shipment
        {
            Id = shipmentId,
            PackageName = "Test Package",
            Weight = 10m,
            Dimensions = new Dimensions { Length = 10, Width = 20, Height = 30 },
            ShippingCost = 50m,
            DestinationAddress = "123 Main St",
            DateCreated = DateTime.UtcNow,
            DateLastUpdated = DateTime.UtcNow,
            Creator = "TestUser",
            Status = "pending"
        };

        var input = new UpdateShipmentStatusInput
        {
            Id = shipmentId.ToString(),
            Status = "in_transit",
            Creator = "TestUser"
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(shipmentId))
            .ReturnsAsync(shipment);

        _repositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Shipment>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess, "Result should fail for repository exception");
        Assert.NotNull(result.Error);
        Assert.Equal("REPOSITORY_ERROR", result.Error!.Code);
        Assert.Equal(500, result.Error.CorrespondingStatusCode);
    }

    // ==================== Logging Verification ====================

    /// <summary>
    /// Test: Logger should log information on successful status update.
    /// </summary>
    [Fact]
    public async Task HandleAsync_Success_ShouldLogInformation()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var shipment = new Shipment
        {
            Id = shipmentId,
            PackageName = "Test Package",
            Weight = 10m,
            Dimensions = new Dimensions { Length = 10, Width = 20, Height = 30 },
            ShippingCost = 50m,
            DestinationAddress = "123 Main St",
            DateCreated = DateTime.UtcNow,
            DateLastUpdated = DateTime.UtcNow,
            Creator = "TestUser",
            Status = "pending"
        };

        var input = new UpdateShipmentStatusInput
        {
            Id = shipmentId.ToString(),
            Status = "in_transit",
            Creator = "TestUser"
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(shipmentId))
            .ReturnsAsync(shipment);

        _repositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Shipment>()))
            .ReturnsAsync((Shipment s) => s);

        // Act
        await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert - Verify logger was called with information level
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    /// <summary>
    /// Test: Logger should log warning on validation error.
    /// </summary>
    [Fact]
    public async Task HandleAsync_ValidationError_ShouldLogWarning()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var input = new UpdateShipmentStatusInput
        {
            Id = shipmentId.ToString(),
            Status = "invalid_status",
            Creator = "TestUser"
        };

        // Act
        await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert - Verify logger was called with warning level
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }
}
