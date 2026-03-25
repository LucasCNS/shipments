using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Shipments.Application.Repositories;
using Shipments.Application.UseCases.ListShipments;
using Shipments.Domain.Models;

namespace Shipments.UnitTests.Application.UseCases;

/// <summary>
/// Unit tests for the ListShipmentsUseCase class.
/// </summary>
public class ListShipmentsUseCaseTests
{
    private readonly Mock<IShipmentRepository> _repositoryMock;
    private readonly Mock<ILogger<ListShipmentsUseCase>> _loggerMock;
    private readonly ListShipmentsUseCase _useCase;

    /// <summary>
    /// Initializes test fixtures.
    /// </summary>
    public ListShipmentsUseCaseTests()
    {
        _repositoryMock = new Mock<IShipmentRepository>();
        _loggerMock = new Mock<ILogger<ListShipmentsUseCase>>();
        _useCase = new ListShipmentsUseCase(_repositoryMock.Object, _loggerMock.Object);
    }

    /// <summary>
    /// Valid input should return ListShipmentsOutput with matching shipments.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithValidInput_ShouldReturnOutput()
    {
        // Arrange
        var input = new ListShipmentsInput
        {
            Status = "pending",
            Limit = 10,
            Offset = 0,
            Creator = "TestUser"
        };

        var shipment1 = new Shipment
        {
            Id = Guid.NewGuid(),
            PackageName = "Package 1",
            Weight = 10m,
            ShippingCost = 50m,
            DestinationAddress = "Address 1",
            Creator = "TestUser",
            Status = "pending",
            DateCreated = DateTime.UtcNow,
            DateLastUpdated = DateTime.UtcNow
        };

        var shipment2 = new Shipment
        {
            Id = Guid.NewGuid(),
            PackageName = "Package 2",
            Weight = 20m,
            ShippingCost = 75m,
            DestinationAddress = "Address 2",
            Creator = "TestUser",
            Status = "pending",
            DateCreated = DateTime.UtcNow,
            DateLastUpdated = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(repo => repo.GetCountAsync("pending", CancellationToken.None))
            .ReturnsAsync(2);

        _repositoryMock
            .Setup(repo => repo.GetAllAsync("pending", 0, 10, CancellationToken.None))
            .ReturnsAsync(new List<Shipment> { shipment1, shipment2 });

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Total);
        Assert.Equal(0, result.Offset);
        Assert.Equal(10, result.Limit);
        Assert.Equal(2, result.Results.Count);
        Assert.Null(result.Error);
        _repositoryMock.Verify(repo => repo.GetCountAsync("pending", CancellationToken.None), Times.Once);
        _repositoryMock.Verify(repo => repo.GetAllAsync("pending", 0, 10, CancellationToken.None), Times.Once);
    }

    /// <summary>
    /// Input with null status should include all statuses.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithNullStatus_ShouldReturnAllShipments()
    {
        // Arrange
        var input = new ListShipmentsInput
        {
            Status = null,
            Limit = 10,
            Offset = 0,
            Creator = "TestUser"
        };

        var shipments = new List<Shipment>
        {
            new Shipment { Id = Guid.NewGuid(), Status = "pending", Creator = "TestUser", PackageName = "P1", DestinationAddress = "A1", DateCreated = DateTime.UtcNow, DateLastUpdated = DateTime.UtcNow },
            new Shipment { Id = Guid.NewGuid(), Status = "in_transit", Creator = "TestUser", PackageName = "P2", DestinationAddress = "A2", DateCreated = DateTime.UtcNow, DateLastUpdated = DateTime.UtcNow }
        };

        _repositoryMock
            .Setup(repo => repo.GetCountAsync(null, CancellationToken.None))
            .ReturnsAsync(2);

        _repositoryMock
            .Setup(repo => repo.GetAllAsync(null, 0, 10, CancellationToken.None))
            .ReturnsAsync(shipments);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Total);
        Assert.Equal(2, result.Results.Count);
        Assert.Null(result.Error);
    }

    /// <summary>
    /// Pagination offset should be applied correctly.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithOffset_ShouldApplyPagination()
    {
        // Arrange
        var input = new ListShipmentsInput
        {
            Status = null,
            Limit = 5,
            Offset = 10,
            Creator = "TestUser"
        };

        var shipments = new List<Shipment>
        {
            new Shipment { Id = Guid.NewGuid(), Creator = "TestUser", PackageName = "P11", DestinationAddress = "A11", DateCreated = DateTime.UtcNow, DateLastUpdated = DateTime.UtcNow },
            new Shipment { Id = Guid.NewGuid(), Creator = "TestUser", PackageName = "P12", DestinationAddress = "A12", DateCreated = DateTime.UtcNow, DateLastUpdated = DateTime.UtcNow },
            new Shipment { Id = Guid.NewGuid(), Creator = "TestUser", PackageName = "P13", DestinationAddress = "A13", DateCreated = DateTime.UtcNow, DateLastUpdated = DateTime.UtcNow }
        };

        _repositoryMock
            .Setup(repo => repo.GetCountAsync(null, CancellationToken.None))
            .ReturnsAsync(100);

        _repositoryMock
            .Setup(repo => repo.GetAllAsync(null, 10, 5, CancellationToken.None))
            .ReturnsAsync(shipments);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(100, result.Total);
        Assert.Equal(10, result.Offset);
        Assert.Equal(5, result.Limit);
        Assert.Equal(3, result.Results.Count);
        _repositoryMock.Verify(repo => repo.GetAllAsync(null, 10, 5, CancellationToken.None), Times.Once);
    }

    /// <summary>
    /// Validation error should return output with error set.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithInvalidInput_ShouldReturnErrorOutput()
    {
        // Arrange
        var input = new ListShipmentsInput
        {
            Status = "invalid_status",
            Limit = 10,
            Offset = 0,
            Creator = "TestUser"
        };

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Error);
        Assert.Equal("VALIDATION_ERROR", result.Error!.Code);
        Assert.Equal(400, result.Error.CorrespondingStatusCode);
        Assert.Empty(result.Results);
        _repositoryMock.Verify(repo => repo.GetCountAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Empty creator should return validation error.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithEmptyCreator_ShouldReturnError()
    {
        // Arrange
        var input = new ListShipmentsInput
        {
            Status = "pending",
            Limit = 10,
            Offset = 0,
            Creator = string.Empty
        };

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Error);
        Assert.Equal("VALIDATION_ERROR", result.Error!.Code);
    }

    /// <summary>
    /// Empty results with valid filters should return zero total.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithNoMatchingShipments_ShouldReturnEmptyResults()
    {
        // Arrange
        var input = new ListShipmentsInput
        {
            Status = "delivered",
            Limit = 10,
            Offset = 0,
            Creator = "TestUser"
        };

        _repositoryMock
            .Setup(repo => repo.GetCountAsync("delivered", CancellationToken.None))
            .ReturnsAsync(0);

        _repositoryMock
            .Setup(repo => repo.GetAllAsync("delivered", 0, 10, CancellationToken.None))
            .ReturnsAsync(new List<Shipment>());

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.Total);
        Assert.Empty(result.Results);
        Assert.Null(result.Error);
    }

    /// <summary>
    /// Should filter by different status values correctly.
    /// </summary>
    [Theory]
    [InlineData("pending")]
    [InlineData("in_transit")]
    [InlineData("delivered")]
    [InlineData("cancelled")]
    public async Task HandleAsync_WithDifferentStatuses_ShouldCallRepositoryWithCorrectStatus(string status)
    {
        // Arrange
        var input = new ListShipmentsInput
        {
            Status = status,
            Limit = 10,
            Offset = 0,
            Creator = "TestUser"
        };

        _repositoryMock
            .Setup(repo => repo.GetCountAsync(status, CancellationToken.None))
            .ReturnsAsync(5);

        _repositoryMock
            .Setup(repo => repo.GetAllAsync(status, 0, 10, CancellationToken.None))
            .ReturnsAsync(new List<Shipment>());

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _repositoryMock.Verify(repo => repo.GetCountAsync(status, CancellationToken.None), Times.Once);
        _repositoryMock.Verify(repo => repo.GetAllAsync(status, 0, 10, CancellationToken.None), Times.Once);
    }

    /// <summary>
    /// Response should preserve the limit from request even if fewer results are returned.
    /// </summary>
    [Fact]
    public async Task HandleAsync_ShouldPreserveLimitInResponse()
    {
        // Arrange
        var input = new ListShipmentsInput
        {
            Status = null,
            Limit = 25,
            Offset = 0,
            Creator = "TestUser"
        };

        var shipments = new List<Shipment>
        {
            new Shipment { Id = Guid.NewGuid(), Creator = "TestUser", PackageName = "P1", DestinationAddress = "A1", DateCreated = DateTime.UtcNow, DateLastUpdated = DateTime.UtcNow }
        };

        _repositoryMock
            .Setup(repo => repo.GetCountAsync(null, CancellationToken.None))
            .ReturnsAsync(1);

        _repositoryMock
            .Setup(repo => repo.GetAllAsync(null, 0, 25, CancellationToken.None))
            .ReturnsAsync(shipments);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(25, result.Limit); // Limit should be 25 as requested
        Assert.Single(result.Results); // Only 1 result returned
        Assert.Equal(1, result.Total);
    }
}

