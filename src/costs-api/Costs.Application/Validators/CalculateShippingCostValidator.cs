using System.Collections.Generic;
using System.Text.RegularExpressions;
using Costs.Application.DTOs;
using Costs.Domain.Results;

namespace Costs.Application.Validators;

/// <summary>
/// Validates input for the CalculateShippingCost use case.
/// </summary>
public static class CalculateShippingCostValidator
{
    private static readonly Regex ZipRegex = new Regex(@"^\d{4,10}(-\d{4})?$", RegexOptions.Compiled);
    private const decimal MaxVolumeCm3 = 15_000m;

    public static Error? Validate(CalculateShippingCostRequest input)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(input.OriginZipCode) || !ZipRegex.IsMatch(input.OriginZipCode))
            errors.Add("OriginZipCode must be between 4 and 10 digits, optionally followed by a hyphen and 4 digits.");

        if (string.IsNullOrWhiteSpace(input.DestinationZipCode) || !ZipRegex.IsMatch(input.DestinationZipCode))
            errors.Add("DestinationZipCode must be between 4 and 10 digits, optionally followed by a hyphen and 4 digits.");

        if (input.Weight <= 0)
            errors.Add("Weight must be greater than zero.");

        if (input.Dimensions == null)
        {
            errors.Add("Dimensions are required.");
        }
        else
        {
            if (input.Dimensions.Length <= 0 || input.Dimensions.Width <= 0 || input.Dimensions.Height <= 0)
                errors.Add("All dimensions (length, width, height) must be greater than zero.");
            else
            {
                var volume = input.Dimensions.Length * input.Dimensions.Width * input.Dimensions.Height;
                if (volume > MaxVolumeCm3)
                    errors.Add($"Package volume ({volume} cm³) exceeds the maximum allowed volume of {MaxVolumeCm3} cm³.");
            }
        }

        if (errors.Count > 0)
        {
            return new Error
            {
                Code = "VALIDATION_ERROR",
                Message = "One or more validation errors occurred.",
                ValidationErrors = errors,
                CorrespondingStatusCode = 400
            };
        }

        return null;
    }
}
