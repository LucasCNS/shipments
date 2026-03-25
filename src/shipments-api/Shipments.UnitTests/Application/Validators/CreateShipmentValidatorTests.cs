using Xunit;
using Shipments.Application.Validators;
using Shipments.Application.UseCases.CreateShipment;
using Shipments.Domain.Models;

namespace Shipments.UnitTests.Application.Validators;

public class CreateShipmentValidatorTests
{
    private static CreateShipmentInput ValidInput() => new CreateShipmentInput
    {
        PackageName = "Test Package",
        Weight = 10.5m,
        Dimensions = new Dimensions { Length = 10, Width = 20, Height = 30 },
        OriginZipCode = "12345",
        DestinationZipCode = "67890",
        DestinationAddress = "123 Main St, City, Country",
        Creator = "TestUser"
    };

    [Fact]
    public void Validate_WithValidData_ShouldReturnNull()
    {
        var result = CreateShipmentValidator.Validate(ValidInput());
        Assert.Null(result);
    }

    [Fact]
    public void Validate_WithEmptyPackageName_ShouldReturnError()
    {
        var input = ValidInput();
        input.PackageName = string.Empty;

        var result = CreateShipmentValidator.Validate(input);

        Assert.NotNull(result);
        Assert.Equal("VALIDATION_ERROR", result!.Code);
        Assert.Equal(400, result.CorrespondingStatusCode);
    }

    [Fact]
    public void Validate_WithNullPackageName_ShouldReturnError()
    {
        var input = ValidInput();
        input.PackageName = null;

        var result = CreateShipmentValidator.Validate(input);

        Assert.NotNull(result);
        Assert.Equal("VALIDATION_ERROR", result!.Code);
    }

    [Fact]
    public void Validate_WithSpecialCharactersInPackageName_ShouldReturnError()
    {
        var input = ValidInput();
        input.PackageName = "Test@Package#123!";

        var result = CreateShipmentValidator.Validate(input);

        Assert.NotNull(result);
        Assert.Equal("VALIDATION_ERROR", result!.Code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5.5)]
    public void Validate_WithInvalidWeight_ShouldReturnError(decimal weight)
    {
        var input = ValidInput();
        input.Weight = weight;

        var result = CreateShipmentValidator.Validate(input);

        Assert.NotNull(result);
        Assert.Equal("VALIDATION_ERROR", result!.Code);
        Assert.Equal(400, result.CorrespondingStatusCode);
    }

    [Fact]
    public void Validate_WithNullDimensions_ShouldReturnError()
    {
        var input = ValidInput();
        input.Dimensions = null;

        var result = CreateShipmentValidator.Validate(input);

        Assert.NotNull(result);
        Assert.Equal("VALIDATION_ERROR", result!.Code);
    }

    [Theory]
    [InlineData(0, 20, 30)]
    [InlineData(10, 0, 30)]
    [InlineData(10, 20, 0)]
    [InlineData(-5, 20, 30)]
    public void Validate_WithInvalidDimensions_ShouldReturnError(decimal length, decimal width, decimal height)
    {
        var input = ValidInput();
        input.Dimensions = new Dimensions { Length = length, Width = width, Height = height };

        var result = CreateShipmentValidator.Validate(input);

        Assert.NotNull(result);
        Assert.Equal("VALIDATION_ERROR", result!.Code);
    }

    [Fact]
    public void Validate_WithNullOriginZipCode_ShouldReturnError()
    {
        var input = ValidInput();
        input.OriginZipCode = null;

        var result = CreateShipmentValidator.Validate(input);

        Assert.NotNull(result);
        Assert.Equal("VALIDATION_ERROR", result!.Code);
        Assert.Equal(400, result.CorrespondingStatusCode);
    }

    [Fact]
    public void Validate_WithNullDestinationZipCode_ShouldReturnError()
    {
        var input = ValidInput();
        input.DestinationZipCode = null;

        var result = CreateShipmentValidator.Validate(input);

        Assert.NotNull(result);
        Assert.Equal("VALIDATION_ERROR", result!.Code);
        Assert.Equal(400, result.CorrespondingStatusCode);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("abc")]
    [InlineData("12345-abc")]
    [InlineData("12345678901")]
    public void Validate_WithInvalidOriginZipCode_ShouldReturnError(string zip)
    {
        var input = ValidInput();
        input.OriginZipCode = zip;

        var result = CreateShipmentValidator.Validate(input);

        Assert.NotNull(result);
        Assert.Equal("VALIDATION_ERROR", result!.Code);
    }

    [Theory]
    [InlineData("1414")]
    [InlineData("12345")]
    [InlineData("12345-6789")]
    [InlineData("1234567890")]
    public void Validate_WithValidZipCode_ShouldReturnNull(string zip)
    {
        var input = ValidInput();
        input.OriginZipCode = zip;
        input.DestinationZipCode = zip;

        var result = CreateShipmentValidator.Validate(input);

        Assert.Null(result);
    }

    [Fact]
    public void Validate_WithEmptyDestinationAddress_ShouldReturnError()
    {
        var input = ValidInput();
        input.DestinationAddress = string.Empty;

        var result = CreateShipmentValidator.Validate(input);

        Assert.NotNull(result);
        Assert.Equal("VALIDATION_ERROR", result!.Code);
    }

    [Fact]
    public void Validate_WithNullDestinationAddress_ShouldReturnError()
    {
        var input = ValidInput();
        input.DestinationAddress = null;

        var result = CreateShipmentValidator.Validate(input);

        Assert.NotNull(result);
        Assert.Equal("VALIDATION_ERROR", result!.Code);
    }

    [Fact]
    public void Validate_WithEmptyCreator_ShouldReturnError()
    {
        var input = ValidInput();
        input.Creator = string.Empty;

        var result = CreateShipmentValidator.Validate(input);

        Assert.NotNull(result);
        Assert.Equal("VALIDATION_ERROR", result!.Code);
    }

    [Fact]
    public void Validate_WithNullCreator_ShouldReturnError()
    {
        var input = ValidInput();
        input.Creator = null;

        var result = CreateShipmentValidator.Validate(input);

        Assert.NotNull(result);
        Assert.Equal("VALIDATION_ERROR", result!.Code);
    }

    [Fact]
    public void Validate_WithMultipleErrors_ShouldReturnAllValidationMessages()
    {
        var input = new CreateShipmentInput
        {
            PackageName = null,
            Weight = 0,
            Dimensions = null,
            OriginZipCode = null,
            DestinationZipCode = null,
            DestinationAddress = null,
            Creator = null
        };

        var result = CreateShipmentValidator.Validate(input);

        Assert.NotNull(result);
        Assert.Equal("VALIDATION_ERROR", result!.Code);
        Assert.True(result.ValidationErrors.Count >= 5);
    }
}
