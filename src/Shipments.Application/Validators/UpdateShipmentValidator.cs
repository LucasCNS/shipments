using System;
using System.Text.RegularExpressions;
using Shipments.Application.UseCases.UpdateShipment;
using Shipments.Domain.Results;

namespace Shipments.Application.Validators;

/// <summary>
/// Validator for UpdateShipment use case input.
/// </summary>
public class UpdateShipmentValidator
{
    /// <summary>
    /// Validates the update shipment input.
    /// </summary>
    /// <param name="input">The input to validate.</param>
    /// <returns>Error if validation fails; null if valid.</returns>
    public static Error? Validate(UpdateShipmentInput input)
    {
        // Validate ID - not empty and valid GUID
        if (string.IsNullOrWhiteSpace(input.Id))
        {
            return new Error
            {
                Code = "INVALID_SHIPMENT_ID",
                Message = "ShipmentId is required and cannot be empty.",
                CorrespondingStatusCode = 400
            };
        }

        if (!Guid.TryParse(input.Id, out _))
        {
            return new Error
            {
                Code = "INVALID_SHIPMENT_ID",
                Message = "ShipmentId must be a valid GUID.",
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

        // Validate PackageName if provided
        if (!string.IsNullOrWhiteSpace(input.PackageName))
        {
            if (ContainsSpecialCharacters(input.PackageName))
            {
                return new Error
                {
                    Code = "INVALID_PACKAGE_NAME",
                    Message = "PackageName contains invalid characters.",
                    CorrespondingStatusCode = 400
                };
            }
        }

        // Validate Weight if provided
        if (input.Weight.HasValue && input.Weight <= 0)
        {
            return new Error
            {
                Code = "INVALID_WEIGHT",
                Message = "Weight must be greater than zero.",
                CorrespondingStatusCode = 400
            };
        }

        // Validate Dimensions if provided
        if (input.Dimensions != null)
        {
            if (input.Dimensions.Length <= 0 || input.Dimensions.Width <= 0 || input.Dimensions.Height <= 0)
            {
                return new Error
                {
                    Code = "INVALID_DIMENSIONS",
                    Message = "All dimensions (length, width, height) must be greater than zero.",
                    CorrespondingStatusCode = 400
                };
            }
        }

        // Validate ShippingCost if provided
        if (input.ShippingCost.HasValue && input.ShippingCost <= 0)
        {
            return new Error
            {
                Code = "INVALID_SHIPPING_COST",
                Message = "ShippingCost must be greater than zero.",
                CorrespondingStatusCode = 400
            };
        }

        // Validate DestinationAddress if provided
        if (!string.IsNullOrWhiteSpace(input.DestinationAddress) && string.IsNullOrWhiteSpace(input.DestinationAddress))
        {
            return new Error
            {
                Code = "INVALID_DESTINATION_ADDRESS",
                Message = "DestinationAddress cannot be empty.",
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
