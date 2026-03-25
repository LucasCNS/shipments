using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Shipments.Application.ExternalServices;
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
    private readonly Mock<IShippingCostServiceClient> _costServiceClientMock;
    private readonly Mock<ILogger<CreateShipmentUseCase>> _loggerMock;
    private readonly CreateShipmentUseCase _useCase;

    public CreateShipmentUseCaseTests()
    {
        _repositoryMock = new Mock<IShipmentRepository>();
        _costServiceClientMock = new Mock<IShippingCostServiceClient>();
        _loggerMock = new Mock<ILogger<CreateShipmentUseCase>>();
        _useCase = new CreateShipmentUseCase(
            _repositoryMock.Object,
            _costServiceClientMock.Object,
            _loggerMock.Object);
    }

    private static CreateShipmentInput ValidInput() => new CreateShipmentInput
    {
        PackageName = "Valid Package",
        Weight = 15.5m,
        Dimensions = new Dimensions { Length = 25, Width = 35, Height = 45 },
        OriginZipCode = "12345",
        DestinationZipCode = "67890",
        DestinationAddress = "456 Oak Ave, Town, Country",
        Creator = "Alice"
    };

    /// <summary>
    /// Valid input should return CreateShipmentOutput with generated UUID.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithValidInput_ShouldReturnOutputWithUuid()
    {
        // Arrange
        var input = ValidInput();
        _costServiceClientMock
            .Setup(c => c.CalculateShippingCostAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(),
                It.IsAny<Dimensions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(100.00m);
        _repositoryMock
            .Setup(repo => repo.CreateAsync(It.IsAny<Shipment>()))
            .ReturnsAsync((Shipment s) => s);

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess, "Result should be successful");
        Assert.NotNull(result.Value);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.Equal("Valid Package", result.Value.PackageName);
        Assert.Equal(15.5m, result.Value.Weight);
        Assert.NotNull(result.Value.Dimensions);
        Assert.Equal(100.00m, result.Value.ShippingCost);
        Assert.Equal("12345", result.Value.OriginZipCode);
        Assert.Equal("67890", result.Value.DestinationZipCode);
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
            OriginZipCode = "12345",
            DestinationZipCode = "67890",
            DestinationAddress = "789 Elm St, Village, Country",
            Creator = "Bob"
        };
        _costServiceClientMock
            .Setup(c => c.CalculateShippingCostAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(),
                It.IsAny<Dimensions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(25.00m);
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
    /// Empty PackageName should return validation error.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithEmptyPackageName_ShouldReturnError()
    {
        // Arrange
        var input = ValidInput();
        input.PackageName = string.Empty;

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("VALIDATION_ERROR", result.Error!.Code);
        Assert.Equal(400, result.Error.CorrespondingStatusCode);
        Assert.True(result.Error.ValidationErrors.Count > 0);
        _repositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Shipment>()), Times.Never);
        _costServiceClientMock.Verify(c => c.CalculateShippingCostAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(),
            It.IsAny<Dimensions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Invalid Weight should return validation error.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithInvalidWeight_ShouldReturnError()
    {
        var input = ValidInput();
        input.Weight = -5.0m;

        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("VALIDATION_ERROR", result.Error!.Code);
        _repositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Shipment>()), Times.Never);
    }

    /// <summary>
    /// Invalid Dimensions should return validation error.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithInvalidDimensions_ShouldReturnError()
    {
        var input = ValidInput();
        input.Dimensions = new Dimensions { Length = 0, Width = 20, Height = 30 };

        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("VALIDATION_ERROR", result.Error!.Code);
        _repositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Shipment>()), Times.Never);
    }

    /// <summary>
    /// Costs API returning null should return 503 error.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WhenCostsApiUnavailable_ShouldReturn503()
    {
        var input = ValidInput();
        _costServiceClientMock
            .Setup(c => c.CalculateShippingCostAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(),
                It.IsAny<Dimensions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((decimal?)null);

        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("COSTS_API_UNAVAILABLE", result.Error!.Code);
        Assert.Equal(503, result.Error.CorrespondingStatusCode);
        _repositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Shipment>()), Times.Never);
    }

    /// <summary>
    /// Empty DestinationAddress should return validation error.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithEmptyDestinationAddress_ShouldReturnError()
    {
        var input = ValidInput();
        input.DestinationAddress = string.Empty;

        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("VALIDATION_ERROR", result.Error!.Code);
        _repositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Shipment>()), Times.Never);
    }

    /// <summary>
    /// Empty Creator should return validation error.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithEmptyCreator_ShouldReturnError()
    {
        var input = ValidInput();
        input.Creator = string.Empty;

        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("VALIDATION_ERROR", result.Error!.Code);
        _repositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Shipment>()), Times.Never);
    }

    /// <summary>
    /// Valid input should call repository exactly once.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithValidInput_ShouldCallRepositoryOnce()
    {
        var input = ValidInput();
        _costServiceClientMock
            .Setup(c => c.CalculateShippingCostAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(),
                It.IsAny<Dimensions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(75.00m);
        _repositoryMock
            .Setup(repo => repo.CreateAsync(It.IsAny<Shipment>()))
            .ReturnsAsync((Shipment s) => s);

        await _useCase.HandleAsync(input, CancellationToken.None);

        _repositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Shipment>()), Times.Once);
    }

    /// <summary>
    /// Special characters in PackageName should return validation error.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithSpecialCharactersInPackageName_ShouldReturnError()
    {
        var input = ValidInput();
        input.PackageName = "Invalid@Package#";

        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("VALIDATION_ERROR", result.Error!.Code);
    }
}
