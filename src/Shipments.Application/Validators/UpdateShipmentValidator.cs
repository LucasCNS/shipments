using System;
using System.Collections.Generic;
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
        var validationErrors = new List<string>();

        // Validate ID - not empty and valid GUID
        if (string.IsNullOrWhiteSpace(input.Id))
        {
            validationErrors.Add("ShipmentId is required and cannot be empty.");
        }
        else if (!Guid.TryParse(input.Id, out _))
        {
            validationErrors.Add("ShipmentId must be a valid GUID.");
        }

        // Validate Creator - not empty
        if (string.IsNullOrWhiteSpace(input.Creator))
        {
            validationErrors.Add("Creator is required and cannot be empty.");
        }

        // Validate PackageName if provided
        if (!string.IsNullOrWhiteSpace(input.PackageName))
        {
            if (ContainsSpecialCharacters(input.PackageName))
            {
                validationErrors.Add("PackageName contains invalid characters.");
            }
        }

        // Validate Weight if provided
        if (input.Weight.HasValue && input.Weight <= 0)
        {
            validationErrors.Add("Weight must be greater than zero.");
        }

        // Validate Dimensions if provided
        if (input.Dimensions != null)
        {
            if (input.Dimensions.Length <= 0 || input.Dimensions.Width <= 0 || input.Dimensions.Height <= 0)
            {
                validationErrors.Add("All dimensions (length, width, height) must be greater than zero.");
            }
        }

        // Validate ShippingCost if provided
        if (input.ShippingCost.HasValue && input.ShippingCost <= 0)
        {
            validationErrors.Add("ShippingCost must be greater than zero.");
        }

        // Validate DestinationAddress if provided
        if (!string.IsNullOrWhiteSpace(input.DestinationAddress) && string.IsNullOrWhiteSpace(input.DestinationAddress))
        {
            validationErrors.Add("DestinationAddress cannot be empty.");
        }

        // Validate at least one data field is provided
        var hasDataFields = !string.IsNullOrWhiteSpace(input.PackageName) ||
                           input.Weight.HasValue ||
                           input.Dimensions != null ||
                           input.ShippingCost.HasValue ||
                           !string.IsNullOrWhiteSpace(input.DestinationAddress);

        if (!hasDataFields)
        {
            validationErrors.Add("At least one field must be provided for update.");
        }

        // Return error if there are any validation errors
        if (validationErrors.Count > 0)
        {
            return new Error
            {
                Code = "VALIDATION_ERROR",
                Message = "One or more validation errors occurred.",
                ValidationErrors = validationErrors,
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
