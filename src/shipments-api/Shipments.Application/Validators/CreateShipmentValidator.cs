using System.Collections.Generic;
using System.Text.RegularExpressions;
using Shipments.Application.UseCases.CreateShipment;
using Shipments.Domain.Results;

namespace Shipments.Application.Validators;

/// <summary>
/// Validator for CreateShipment use case input.
/// </summary>
public class CreateShipmentValidator
{
    private static readonly Regex ZipRegex = new Regex(@"^\d{4,10}(-\d{4})?$", RegexOptions.Compiled);

    /// <summary>
    /// Validates the create shipment input.
    /// </summary>
    /// <param name="input">The input to validate.</param>
    /// <returns>Error if validation fails; null if valid.</returns>
    public static Error? Validate(CreateShipmentInput input)
    {
        var validationErrors = new List<string>();

        // Validate PackageName - not empty and no special characters
        if (string.IsNullOrWhiteSpace(input.PackageName))
        {
            validationErrors.Add("PackageName is required and cannot be empty.");
        }
        else if (ContainsSpecialCharacters(input.PackageName))
        {
            validationErrors.Add("PackageName contains invalid characters.");
        }

        // Validate Weight > 0
        if (input.Weight <= 0)
        {
            validationErrors.Add("Weight must be greater than zero.");
        }

        // Validate Dimensions - all > 0
        if (input.Dimensions == null)
        {
            validationErrors.Add("Dimensions are required.");
        }
        else if (input.Dimensions.Length <= 0 || input.Dimensions.Width <= 0 || input.Dimensions.Height <= 0)
        {
            validationErrors.Add("All dimensions (length, width, height) must be greater than zero.");
        }

        // Validate OriginZipCode
        if (string.IsNullOrWhiteSpace(input.OriginZipCode) || !ZipRegex.IsMatch(input.OriginZipCode))
            validationErrors.Add("OriginZipCode must be between 4 and 10 digits, optionally followed by a hyphen and 4 digits.");

        // Validate DestinationZipCode
        if (string.IsNullOrWhiteSpace(input.DestinationZipCode) || !ZipRegex.IsMatch(input.DestinationZipCode))
            validationErrors.Add("DestinationZipCode must be between 4 and 10 digits, optionally followed by a hyphen and 4 digits.");

        // Validate DestinationAddress - not empty
        if (string.IsNullOrWhiteSpace(input.DestinationAddress))
        {
            validationErrors.Add("DestinationAddress is required and cannot be empty.");
        }

        // Validate Creator - not empty
        if (string.IsNullOrWhiteSpace(input.Creator))
        {
            validationErrors.Add("Creator is required and cannot be empty.");
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
