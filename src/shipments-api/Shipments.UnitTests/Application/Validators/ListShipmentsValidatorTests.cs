using Xunit;
using Shipments.Application.Validators;
using Shipments.Application.UseCases.ListShipments;

namespace Shipments.UnitTests.Application.Validators;

/// <summary>
/// Unit tests for the ListShipmentsValidator class.
/// </summary>
public class ListShipmentsValidatorTests
{
    /// <summary>
    /// Valid list shipments data should return null (no error).
    /// </summary>
    [Fact]
    public void Validate_WithValidData_ShouldReturnNull()
    {
        // Arrange
        var input = new ListShipmentsInput
        {
            Status = "pending",
            Limit = 10,
            Offset = 0,
            Creator = "TestUser"
        };

        // Act
        var result = ListShipmentsValidator.Validate(input);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Valid list shipments data without status should return null.
    /// </summary>
    [Fact]
    public void Validate_WithNullStatus_ShouldReturnNull()
    {
        // Arrange
        var input = new ListShipmentsInput
        {
            Status = null,
            Limit = 10,
            Offset = 0,
            Creator = "TestUser"
        };

        // Act
        var result = ListShipmentsValidator.Validate(input);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Valid status values should all pass validation.
    /// </summary>
    [Theory]
    [InlineData("pending")]
    [InlineData("in_transit")]
    [InlineData("delivered")]
    [InlineData("cancelled")]
    public void Validate_WithValidStatus_ShouldReturnNull(string status)
    {
        // Arrange
        var input = new ListShipmentsInput
        {
            Status = status,
            Limit = 10,
            Offset = 0,
            Creator = "TestUser"
        };

        // Act
        var result = ListShipmentsValidator.Validate(input);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Invalid status should return error.
    /// </summary>
    [Fact]
    public void Validate_WithInvalidStatus_ShouldReturnError()
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
        var result = ListShipmentsValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("INVALID_STATUS", result!.Code);
        Assert.Equal(400, result.CorrespondingStatusCode);
    }

    /// <summary>
    /// Empty Creator should return error.
    /// </summary>
    [Fact]
    public void Validate_WithEmptyCreator_ShouldReturnError()
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
        var result = ListShipmentsValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("EMPTY_CREATOR", result!.Code);
        Assert.Equal(400, result.CorrespondingStatusCode);
    }

    /// <summary>
    /// Null Creator should return error.
    /// </summary>
    [Fact]
    public void Validate_WithNullCreator_ShouldReturnError()
    {
        // Arrange
        var input = new ListShipmentsInput
        {
            Status = "pending",
            Limit = 10,
            Offset = 0,
            Creator = null
        };

        // Act
        var result = ListShipmentsValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("EMPTY_CREATOR", result!.Code);
    }

    /// <summary>
    /// Negative Offset should return error.
    /// </summary>
    [Fact]
    public void Validate_WithNegativeOffset_ShouldReturnError()
    {
        // Arrange
        var input = new ListShipmentsInput
        {
            Status = "pending",
            Limit = 10,
            Offset = -1,
            Creator = "TestUser"
        };

        // Act
        var result = ListShipmentsValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("INVALID_OFFSET", result!.Code);
        Assert.Equal(400, result.CorrespondingStatusCode);
    }

    /// <summary>
    /// Offset of 0 should be valid.
    /// </summary>
    [Fact]
    public void Validate_WithOffsetZero_ShouldReturnNull()
    {
        // Arrange
        var input = new ListShipmentsInput
        {
            Status = "pending",
            Limit = 10,
            Offset = 0,
            Creator = "TestUser"
        };

        // Act
        var result = ListShipmentsValidator.Validate(input);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Limit of 0 or less should return error.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_WithZeroOrNegativeLimit_ShouldReturnError(int limit)
    {
        // Arrange
        var input = new ListShipmentsInput
        {
            Status = "pending",
            Limit = limit,
            Offset = 0,
            Creator = "TestUser"
        };

        // Act
        var result = ListShipmentsValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("INVALID_LIMIT", result!.Code);
        Assert.Equal(400, result.CorrespondingStatusCode);
    }

    /// <summary>
    /// Limit exceeding 100 should return error.
    /// </summary>
    [Fact]
    public void Validate_WithLimitGreaterThan100_ShouldReturnError()
    {
        // Arrange
        var input = new ListShipmentsInput
        {
            Status = "pending",
            Limit = 101,
            Offset = 0,
            Creator = "TestUser"
        };

        // Act
        var result = ListShipmentsValidator.Validate(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("LIMIT_EXCEEDED", result!.Code);
        Assert.Equal(400, result.CorrespondingStatusCode);
    }

    /// <summary>
    /// Limit of 100 should be valid (maximum allowed).
    /// </summary>
    [Fact]
    public void Validate_WithLimitOf100_ShouldReturnNull()
    {
        // Arrange
        var input = new ListShipmentsInput
        {
            Status = "pending",
            Limit = 100,
            Offset = 0,
            Creator = "TestUser"
        };

        // Act
        var result = ListShipmentsValidator.Validate(input);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Status comparison should be case-insensitive.
    /// </summary>
    [Theory]
    [InlineData("PENDING")]
    [InlineData("Pending")]
    [InlineData("IN_TRANSIT")]
    public void Validate_WithMixedCaseStatus_ShouldReturnNull(string status)
    {
        // Arrange
        var input = new ListShipmentsInput
        {
            Status = status,
            Limit = 10,
            Offset = 0,
            Creator = "TestUser"
        };

        // Act
        var result = ListShipmentsValidator.Validate(input);

        // Assert
        Assert.Null(result);
    }
}
