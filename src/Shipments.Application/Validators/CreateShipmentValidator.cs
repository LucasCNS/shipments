using System.Text.RegularExpressions;
using Shipments.Application.UseCases.CreateShipment;
using Shipments.Domain.Results;

namespace Shipments.Application.Validators;

/// <summary>
/// Validator for CreateShipment use case input.
/// </summary>
public class CreateShipmentValidator
{
    /// <summary>
    /// Validates the create shipment input.
    /// </summary>
    /// <param name="input">The input to validate.</param>
    /// <returns>Error if validation fails; null if valid.</returns>
    public static Error? Validate(CreateShipmentInput input)
    {
        // Validate PackageName - not empty and no special characters
        if (string.IsNullOrWhiteSpace(input.PackageName))
        {
            return new Error
            {
                Code = "EMPTY_PACKAGE_NAME",
                Message = "PackageName is required and cannot be empty.",
                CorrespondingStatusCode = 400
            };
        }

        if (ContainsSpecialCharacters(input.PackageName))
        {
            return new Error
            {
                Code = "INVALID_PACKAGE_NAME",
                Message = "PackageName contains invalid characters.",
                CorrespondingStatusCode = 400
            };
        }

        // Validate Weight > 0
        if (input.Weight <= 0)
        {
            return new Error
            {
                Code = "INVALID_WEIGHT",
                Message = "Weight must be greater than zero.",
                CorrespondingStatusCode = 400
            };
        }

        // Validate Dimensions - all > 0
        if (input.Dimensions == null)
        {
            return new Error
            {
                Code = "MISSING_DIMENSIONS",
                Message = "Dimensions are required.",
                CorrespondingStatusCode = 400
            };
        }

        if (input.Dimensions.Length <= 0 || input.Dimensions.Width <= 0 || input.Dimensions.Height <= 0)
        {
            return new Error
            {
                Code = "INVALID_DIMENSIONS",
                Message = "All dimensions (length, width, height) must be greater than zero.",
                CorrespondingStatusCode = 400
            };
        }

        // Validate ShippingCost > 0
        if (input.ShippingCost <= 0)
        {
            return new Error
            {
                Code = "INVALID_SHIPPING_COST",
                Message = "ShippingCost must be greater than zero.",
                CorrespondingStatusCode = 400
            };
        }

        // Validate DestinationAddress - not empty
        if (string.IsNullOrWhiteSpace(input.DestinationAddress))
        {
            return new Error
            {
                Code = "EMPTY_DESTINATION_ADDRESS",
                Message = "DestinationAddress is required and cannot be empty.",
                CorrespondingStatusCode = 400
            };
        }

        // Validate Creator - not empty
        if (string.IsNullOrWhiteSpace(input.Creator))
        {
            return new Error
            {
                Code = "EMPTY_CREATOR",
                Message = "Creator is required and cannot be empty.",
                CorrespondingStatusCode = 400
            };
        }

        return null; // Valid
    }

    /// <summary>
    /// Checks if a string contains special characters.
    /// </summary>
    private static bool ContainsSpecialCharacters(string value)
    {
        // Allow alphanumeric, spaces, hyphens, and underscores
        return !Regex.IsMatch(value, @"^[a-zA-Z0-9\s\-_]+$");
    }
}
