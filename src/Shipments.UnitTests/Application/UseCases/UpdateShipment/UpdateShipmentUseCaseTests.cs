using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Shipments.Application.Repositories;
using Shipments.Application.UseCases.UpdateShipment;
using Shipments.Domain.Models;

namespace Shipments.UnitTests.Application.UseCases;

/// <summary>
/// Unit tests for the UpdateShipmentUseCase class.
/// </summary>
public class UpdateShipmentUseCaseTests
{
    private readonly Mock<IShipmentRepository> _repositoryMock;
    private readonly Mock<ILogger<UpdateShipmentUseCase>> _loggerMock;
    private readonly UpdateShipmentUseCase _useCase;

    /// <summary>
    /// Initializes test fixtures.
    /// </summary>
    public UpdateShipmentUseCaseTests()
    {
        _repositoryMock = new Mock<IShipmentRepository>();
        _loggerMock = new Mock<ILogger<UpdateShipmentUseCase>>();
        _useCase = new UpdateShipmentUseCase(_repositoryMock.Object, _loggerMock.Object);
    }

    /// <summary>
    /// Valid input should update and return shipment with updated fields.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithValidInput_ShouldReturnUpdatedShipment()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var existingShipment = new Shipment
        {
            Id = shipmentId,
            PackageName = "Original Package",
            Weight = 5m,
            Dimensions = new Dimensions { Length = 10, Width = 10, Height = 10 },
            ShippingCost = 100m,
            DestinationAddress = "Original Address",
            DateCreated = DateTime.UtcNow.AddDays(-1),
            DateLastUpdated = DateTime.UtcNow.AddDays(-1),
            Creator = "Creator1",
            Status = "pending"
        };

        var input = new UpdateShipmentInput
        {
            Id = shipmentId.ToString(),
            Creator = "Creator1",
            PackageName = "Updated Package",
            Weight = 10m
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(shipmentId))
            .ReturnsAsync(existingShipment);

        _repositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Shipment>()))
            .ReturnsAsync((Shipment s) => s);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Error);
        Assert.Equal(shipmentId, result.Id);
        Assert.Equal("Updated Package", result.PackageName);
        Assert.Equal(10m, result.Weight);
        Assert.Equal("pending", result.Status);
        _repositoryMock.Verify(repo => repo.GetByIdAsync(shipmentId), Times.Once);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Shipment>()), Times.Once);
    }

    /// <summary>
    /// Should return error 400 when validation fails.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithInvalidInput_ShouldReturnValidationError()
    {
        // Arrange
        var input = new UpdateShipmentInput
        {
            Id = "invalid-guid",
            Creator = "Creator1"
        };

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Error);
        Assert.Equal("INVALID_SHIPMENT_ID", result.Error.Code);
        Assert.Equal(400, result.Error.CorrespondingStatusCode);
        _repositoryMock.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    /// <summary>
    /// Should return error 404 when shipment does not exist.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithNonExistentShipment_ShouldReturnNotFoundError()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var input = new UpdateShipmentInput
        {
            Id = shipmentId.ToString(),
            Creator = "Creator1",
            PackageName = "Updated Package"
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(shipmentId))
            .ReturnsAsync((Shipment?)null);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Error);
        Assert.Equal("SHIPMENT_NOT_FOUND", result.Error.Code);
        Assert.Equal(404, result.Error.CorrespondingStatusCode);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Shipment>()), Times.Never);
    }

    /// <summary>
    /// Should return error 409 when shipment is not in pending status.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithNonPendingShipment_ShouldReturnConflictError()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var existingShipment = new Shipment
        {
            Id = shipmentId,
            PackageName = "Package",
            Weight = 5m,
            Dimensions = new Dimensions { Length = 10, Width = 10, Height = 10 },
            ShippingCost = 100m,
            DestinationAddress = "Address",
            DateCreated = DateTime.UtcNow,
            DateLastUpdated = DateTime.UtcNow,
            Creator = "Creator1",
            Status = "in_transit"
        };

        var input = new UpdateShipmentInput
        {
            Id = shipmentId.ToString(),
            Creator = "Creator1",
            PackageName = "Updated Package"
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(shipmentId))
            .ReturnsAsync(existingShipment);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Error);
        Assert.Equal("SHIPMENT_NOT_UPDATABLE", result.Error.Code);
        Assert.Equal(409, result.Error.CorrespondingStatusCode);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Shipment>()), Times.Never);
    }

    /// <summary>
    /// Should return error when no fields are provided for update.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithNoFieldsToUpdate_ShouldReturnError()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var existingShipment = new Shipment
        {
            Id = shipmentId,
            PackageName = "Package",
            Weight = 5m,
            Dimensions = new Dimensions { Length = 10, Width = 10, Height = 10 },
            ShippingCost = 100m,
            DestinationAddress = "Address",
            DateCreated = DateTime.UtcNow,
            DateLastUpdated = DateTime.UtcNow,
            Creator = "Creator1",
            Status = "pending"
        };

        var input = new UpdateShipmentInput
        {
            Id = shipmentId.ToString(),
            Creator = "Creator1"
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(shipmentId))
            .ReturnsAsync(existingShipment);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Error);
        Assert.Equal("NO_FIELDS_TO_UPDATE", result.Error.Code);
        Assert.Equal(400, result.Error.CorrespondingStatusCode);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Shipment>()), Times.Never);
    }

    /// <summary>
    /// Should update only provided fields and maintain others.
    /// </summary>
    [Fact]
    public async Task HandleAsync_ShouldUpdateOnlyProvidedFields()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var originalDestination = "Original Address";
        var existingShipment = new Shipment
        {
            Id = shipmentId,
            PackageName = "Original Package",
            Weight = 5m,
            Dimensions = new Dimensions { Length = 10, Width = 10, Height = 10 },
            ShippingCost = 100m,
            DestinationAddress = originalDestination,
            DateCreated = DateTime.UtcNow.AddDays(-1),
            DateLastUpdated = DateTime.UtcNow.AddDays(-1),
            Creator = "Creator1",
            Status = "pending"
        };

        var input = new UpdateShipmentInput
        {
            Id = shipmentId.ToString(),
            Creator = "Creator1",
            PackageName = "Updated Package"
            // Weight, Dimensions, ShippingCost, DestinationAddress not provided
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(shipmentId))
            .ReturnsAsync(existingShipment);

        _repositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Shipment>()))
            .ReturnsAsync((Shipment s) => s);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Package", result.PackageName);
        Assert.Equal(5m, result.Weight); // Should remain unchanged
        Assert.Equal(originalDestination, result.DestinationAddress); // Should remain unchanged
    }

    /// <summary>
    /// Should update DateLastUpdated to current UTC time.
    /// </summary>
    [Fact]
    public async Task HandleAsync_ShouldUpdateDateLastUpdated()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var oldDateTime = DateTime.UtcNow.AddHours(-1);
        var existingShipment = new Shipment
        {
            Id = shipmentId,
            PackageName = "Package",
            Weight = 5m,
            Dimensions = new Dimensions { Length = 10, Width = 10, Height = 10 },
            ShippingCost = 100m,
            DestinationAddress = "Address",
            DateCreated = oldDateTime,
            DateLastUpdated = oldDateTime,
            Creator = "Creator1",
            Status = "pending"
        };

        var input = new UpdateShipmentInput
        {
            Id = shipmentId.ToString(),
            Creator = "Creator1",
            Weight = 15m
        };

        var beforeUpdate = DateTime.UtcNow;
        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(shipmentId))
            .ReturnsAsync(existingShipment);

        _repositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Shipment>()))
            .ReturnsAsync((Shipment s) => s);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);
        var afterUpdate = DateTime.UtcNow;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.DateLastUpdated >= beforeUpdate && result.DateLastUpdated <= afterUpdate);
        Assert.True(result.DateLastUpdated > oldDateTime);
    }

    /// <summary>
    /// Should call repository UpdateAsync with correct object.
    /// </summary>
    [Fact]
    public async Task HandleAsync_ShouldCallRepositoryUpdateAsync()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var existingShipment = new Shipment
        {
            Id = shipmentId,
            PackageName = "Package",
            Weight = 5m,
            Dimensions = new Dimensions { Length = 10, Width = 10, Height = 10 },
            ShippingCost = 100m,
            DestinationAddress = "Address",
            DateCreated = DateTime.UtcNow,
            DateLastUpdated = DateTime.UtcNow,
            Creator = "Creator1",
            Status = "pending"
        };

        var input = new UpdateShipmentInput
        {
            Id = shipmentId.ToString(),
            Creator = "Creator1",
            Weight = 15m
        };

        var capturedShipment = (Shipment?)null;
        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(shipmentId))
            .ReturnsAsync(existingShipment);

        _repositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Shipment>()))
            .Callback<Shipment>(s => capturedShipment = s)
            .ReturnsAsync((Shipment s) => s);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(capturedShipment);
        Assert.Equal(shipmentId, capturedShipment.Id);
        Assert.Equal(15m, capturedShipment.Weight);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Shipment>()), Times.Once);
    }

    /// <summary>
    /// Should return output with all shipment data and null error on success.
    /// </summary>
    [Fact]
    public async Task HandleAsync_OnSuccess_ShouldReturnCompleteOutput()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var existingShipment = new Shipment
        {
            Id = shipmentId,
            PackageName = "Original Package",
            Weight = 5m,
            Dimensions = new Dimensions { Length = 10, Width = 10, Height = 10 },
            ShippingCost = 100m,
            DestinationAddress = "Address",
            DateCreated = DateTime.UtcNow,
            DateLastUpdated = DateTime.UtcNow,
            Creator = "Creator1",
            Status = "pending"
        };

        var input = new UpdateShipmentInput
        {
            Id = shipmentId.ToString(),
            Creator = "Creator1",
            PackageName = "Updated Package"
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(shipmentId))
            .ReturnsAsync(existingShipment);

        _repositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Shipment>()))
            .ReturnsAsync((Shipment s) => s);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Error);
        Assert.Equal(shipmentId, result.Id);
        Assert.Equal("Updated Package", result.PackageName);
        Assert.Equal(5m, result.Weight);
        Assert.NotNull(result.Dimensions);
        Assert.Equal(100m, result.ShippingCost);
        Assert.Equal("Address", result.DestinationAddress);
        Assert.Equal("Creator1", result.Creator);
        Assert.Equal("pending", result.Status);
    }

    // ===== Status transition tests =====

    /// <summary>
    /// PUT with status = "in_transit" on a pending shipment should succeed.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithStatusInTransitOnPendingShipment_ShouldSucceed()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var existingShipment = new Shipment
        {
            Id = shipmentId,
            PackageName = "Package",
            Weight = 5m,
            Dimensions = new Dimensions { Length = 10, Width = 10, Height = 10 },
            ShippingCost = 100m,
            DestinationAddress = "Address",
            DateCreated = DateTime.UtcNow,
            DateLastUpdated = DateTime.UtcNow,
            Creator = "Creator1",
            Status = "pending"
        };

        var input = new UpdateShipmentInput
        {
            Id = shipmentId.ToString(),
            Creator = "Creator1",
            Status = "in_transit"
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(shipmentId))
            .ReturnsAsync(existingShipment);

        _repositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Shipment>()))
            .ReturnsAsync((Shipment s) => s);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Error);
        Assert.Equal("in_transit", result.Status);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Shipment>()), Times.Once);
    }

    /// <summary>
    /// PUT with status = "cancelled" on a pending shipment should succeed.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithStatusCancelledOnPendingShipment_ShouldSucceed()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var existingShipment = new Shipment
        {
            Id = shipmentId,
            PackageName = "Package",
            Weight = 5m,
            Dimensions = new Dimensions { Length = 10, Width = 10, Height = 10 },
            ShippingCost = 100m,
            DestinationAddress = "Address",
            DateCreated = DateTime.UtcNow,
            DateLastUpdated = DateTime.UtcNow,
            Creator = "Creator1",
            Status = "pending"
        };

        var input = new UpdateShipmentInput
        {
            Id = shipmentId.ToString(),
            Creator = "Creator1",
            Status = "cancelled"
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(shipmentId))
            .ReturnsAsync(existingShipment);

        _repositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Shipment>()))
            .ReturnsAsync((Shipment s) => s);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Error);
        Assert.Equal("cancelled", result.Status);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Shipment>()), Times.Once);
    }

    /// <summary>
    /// PUT with status = "delivered" on an in_transit shipment should succeed.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithStatusDeliveredOnInTransitShipment_ShouldSucceed()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var existingShipment = new Shipment
        {
            Id = shipmentId,
            PackageName = "Package",
            Weight = 5m,
            Dimensions = new Dimensions { Length = 10, Width = 10, Height = 10 },
            ShippingCost = 100m,
            DestinationAddress = "Address",
            DateCreated = DateTime.UtcNow,
            DateLastUpdated = DateTime.UtcNow,
            Creator = "Creator1",
            Status = "in_transit"
        };

        var input = new UpdateShipmentInput
        {
            Id = shipmentId.ToString(),
            Creator = "Creator1",
            Status = "delivered"
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(shipmentId))
            .ReturnsAsync(existingShipment);

        _repositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Shipment>()))
            .ReturnsAsync((Shipment s) => s);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Error);
        Assert.Equal("delivered", result.Status);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Shipment>()), Times.Once);
    }

    /// <summary>
    /// PUT with status = "delivered" on a pending shipment should return error 409 (invalid transition).
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithStatusDeliveredOnPendingShipment_ShouldReturnError409()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var existingShipment = new Shipment
        {
            Id = shipmentId,
            PackageName = "Package",
            Weight = 5m,
            Dimensions = new Dimensions { Length = 10, Width = 10, Height = 10 },
            ShippingCost = 100m,
            DestinationAddress = "Address",
            DateCreated = DateTime.UtcNow,
            DateLastUpdated = DateTime.UtcNow,
            Creator = "Creator1",
            Status = "pending"
        };

        var input = new UpdateShipmentInput
        {
            Id = shipmentId.ToString(),
            Creator = "Creator1",
            Status = "delivered"
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(shipmentId))
            .ReturnsAsync(existingShipment);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Error);
        Assert.Equal("INVALID_STATUS_TRANSITION", result.Error.Code);
        Assert.Equal(409, result.Error.CorrespondingStatusCode);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Shipment>()), Times.Never);
    }

    /// <summary>
    /// PUT with data fields on an in_transit shipment (without status change) should return error 409.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithDataFieldsOnInTransitShipment_ShouldReturnError409()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var existingShipment = new Shipment
        {
            Id = shipmentId,
            PackageName = "Package",
            Weight = 5m,
            Dimensions = new Dimensions { Length = 10, Width = 10, Height = 10 },
            ShippingCost = 100m,
            DestinationAddress = "Address",
            DateCreated = DateTime.UtcNow,
            DateLastUpdated = DateTime.UtcNow,
            Creator = "Creator1",
            Status = "in_transit"
        };

        var input = new UpdateShipmentInput
        {
            Id = shipmentId.ToString(),
            Creator = "Creator1",
            PackageName = "Updated Package"
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(shipmentId))
            .ReturnsAsync(existingShipment);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Error);
        Assert.Equal("SHIPMENT_NOT_UPDATABLE", result.Error.Code);
        Assert.Equal(409, result.Error.CorrespondingStatusCode);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Shipment>()), Times.Never);
    }

    /// <summary>
    /// PUT with status = "in_transit" and data fields on a pending shipment should succeed (both allowed).
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithStatusAndDataFieldsOnPendingShipment_ShouldSucceed()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var existingShipment = new Shipment
        {
            Id = shipmentId,
            PackageName = "Original Package",
            Weight = 5m,
            Dimensions = new Dimensions { Length = 10, Width = 10, Height = 10 },
            ShippingCost = 100m,
            DestinationAddress = "Address",
            DateCreated = DateTime.UtcNow,
            DateLastUpdated = DateTime.UtcNow,
            Creator = "Creator1",
            Status = "pending"
        };

        var input = new UpdateShipmentInput
        {
            Id = shipmentId.ToString(),
            Creator = "Creator1",
            PackageName = "Updated Package",
            Status = "in_transit"
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(shipmentId))
            .ReturnsAsync(existingShipment);

        _repositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Shipment>()))
            .ReturnsAsync((Shipment s) => s);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Error);
        Assert.Equal("Updated Package", result.PackageName);
        Assert.Equal("in_transit", result.Status);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Shipment>()), Times.Once);
    }

    /// <summary>
    /// PUT with status = "pending" on a delivered shipment should return error 409 (final state).
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithStatusPendingOnDeliveredShipment_ShouldReturnError409()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var existingShipment = new Shipment
        {
            Id = shipmentId,
            PackageName = "Package",
            Weight = 5m,
            Dimensions = new Dimensions { Length = 10, Width = 10, Height = 10 },
            ShippingCost = 100m,
            DestinationAddress = "Address",
            DateCreated = DateTime.UtcNow,
            DateLastUpdated = DateTime.UtcNow,
            Creator = "Creator1",
            Status = "delivered"
        };

        var input = new UpdateShipmentInput
        {
            Id = shipmentId.ToString(),
            Creator = "Creator1",
            Status = "pending"
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(shipmentId))
            .ReturnsAsync(existingShipment);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Error);
        Assert.Equal("INVALID_STATUS_TRANSITION", result.Error.Code);
        Assert.Equal(409, result.Error.CorrespondingStatusCode);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Shipment>()), Times.Never);
    }
}
