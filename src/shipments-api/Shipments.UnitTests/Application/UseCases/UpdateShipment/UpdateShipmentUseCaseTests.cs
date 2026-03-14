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
/// Tests verify that: Status updates are NOT supported, but data field updates are allowed only when status = "pending".
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

    // ===== Scenario 1: Successful update when pending =====

    /// <summary>
    /// Test 1: Should successfully update data fields when shipment is pending.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithValidDataFieldsOnPendingShipment_ShouldSucceed()
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
            Weight = 10m,
            DestinationAddress = "Updated Address"
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
        Assert.Equal("Updated Address", result.DestinationAddress);
        Assert.Equal("pending", result.Status); // Status should NOT change
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Shipment>()), Times.Once);
    }

    // ===== Scenario 2: Update blocked when not pending =====

    /// <summary>
    /// Test 2: Should return error 409 when trying to update data fields on non-pending shipment.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithDataFieldsOnNonPendingShipment_ShouldReturnConflictError()
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

    // ===== Scenario 3: Status in input is ignored =====

    /// <summary>
    /// Test 3: Status field in input should be ignored (not updated).
    /// </summary>
    [Fact]
    public async Task HandleAsync_StatusPropertyRemoved_StatusShouldNotBeUpdatable()
    {
        // Arrange - Status property is removed from UpdateShipmentInput, 
        // so this test verifies the removal is effective
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
            // Status property is NOT available in UpdateShipmentInput
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
        Assert.Equal("pending", result.Status); // Status should remain unchanged
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Shipment>()), Times.Once);
    }

    // ===== Scenario 4: Error when no fields provided =====

    /// <summary>
    /// Test 4: Should return error 400 when no fields are provided for update.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithNoFieldsToUpdate_ShouldReturnValidationError()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var input = new UpdateShipmentInput
        {
            Id = shipmentId.ToString(),
            Creator = "Creator1"
            // No data fields provided
        };

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Error);
        Assert.Equal("VALIDATION_ERROR", result.Error.Code);
        Assert.Equal(400, result.Error.CorrespondingStatusCode);
        _repositoryMock.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Shipment>()), Times.Never);
    }

    // ===== Scenario 5: Update individual data fields =====

    /// <summary>
    /// Test 5a: Should successfully update PackageName only.
    /// </summary>
    [Fact]
    public async Task HandleAsync_UpdatePackageNameOnly_ShouldSucceed()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var originalWeight = 5m;
        var originalCost = 100m;
        var existingShipment = new Shipment
        {
            Id = shipmentId,
            PackageName = "Original",
            Weight = originalWeight,
            Dimensions = new Dimensions { Length = 10, Width = 10, Height = 10 },
            ShippingCost = originalCost,
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
            PackageName = "Updated"
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
        Assert.Equal("Updated", result.PackageName);
        Assert.Equal(originalWeight, result.Weight); // Should remain unchanged
        Assert.Equal(originalCost, result.ShippingCost); // Should remain unchanged
    }

    /// <summary>
    /// Test 5b: Should successfully update Weight only.
    /// </summary>
    [Fact]
    public async Task HandleAsync_UpdateWeightOnly_ShouldSucceed()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var originalPackageName = "Package";
        var existingShipment = new Shipment
        {
            Id = shipmentId,
            PackageName = originalPackageName,
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
        Assert.Equal(15m, result.Weight);
        Assert.Equal(originalPackageName, result.PackageName); // Should remain unchanged
    }

    /// <summary>
    /// Test 5c: Should successfully update Dimensions only.
    /// </summary>
    [Fact]
    public async Task HandleAsync_UpdateDimensionsOnly_ShouldSucceed()
    {
        // Arrange
        var shipmentId = Guid.NewGuid();
        var originalWeight = 5m;
        var originalPackageName = "Package";
        var existingShipment = new Shipment
        {
            Id = shipmentId,
            PackageName = originalPackageName,
            Weight = originalWeight,
            Dimensions = new Dimensions { Length = 10, Width = 10, Height = 10 },
            ShippingCost = 100m,
            DestinationAddress = "Address",
            DateCreated = DateTime.UtcNow,
            DateLastUpdated = DateTime.UtcNow,
            Creator = "Creator1",
            Status = "pending"
        };

        var newDimensions = new Dimensions { Length = 20, Width = 15, Height = 12 };
        var input = new UpdateShipmentInput
        {
            Id = shipmentId.ToString(),
            Creator = "Creator1",
            Dimensions = newDimensions
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
        Assert.NotNull(result.Dimensions);
        Assert.Equal(20, result.Dimensions.Length);
        Assert.Equal(15, result.Dimensions.Width);
        Assert.Equal(12, result.Dimensions.Height);
        Assert.Equal(originalWeight, result.Weight); // Should remain unchanged
        Assert.Equal(originalPackageName, result.PackageName); // Should remain unchanged
    }

    // ===== Scenario 6: Shipment not found =====

    /// <summary>
    /// Test 6: Should return error 404 when shipment is not found.
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

    // ===== Scenario 7: Invalid GUID =====

    /// <summary>
    /// Test 7: Should return error 400 when ShipmentId is not a valid GUID.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithInvalidGuid_ShouldReturnBadRequestError()
    {
        // Arrange
        var input = new UpdateShipmentInput
        {
            Id = "not-a-guid",
            Creator = "Creator1",
            PackageName = "Updated Package"
        };

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Error);
        Assert.Equal("INVALID_SHIPMENT_ID", result.Error.Code);
        Assert.Equal(400, result.Error.CorrespondingStatusCode);
        _repositoryMock.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Shipment>()), Times.Never);
    }

    // ===== Additional tests =====

    /// <summary>
    /// Additional: Should update DateLastUpdated to current UTC time.
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
    /// Additional: Should return output with all shipment data and null error on success.
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
}
