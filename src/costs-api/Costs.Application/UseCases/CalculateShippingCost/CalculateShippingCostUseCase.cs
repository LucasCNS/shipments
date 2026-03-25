using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Costs.Application.DTOs;
using Costs.Application.Results;
using Costs.Application.Validators;

namespace Costs.Application.UseCases.CalculateShippingCost;

/// <summary>
/// Calculates shipping cost based on origin/destination ZIP codes, weight, and dimensions.
/// </summary>
public class CalculateShippingCostUseCase : ICalculateShippingCostUseCase
{
    // Cost constants
    private const decimal CostPerMile = 0.50m;
    private const decimal WeightSurchargePerHalfKg = 0.10m;
    private const decimal VolumeSurchargePerCm3 = 0.000005m;
    private const decimal ExpressSurchargeMultiplier = 0.50m;

    public Task<Result<CalculateShippingCostResponse>> HandleAsync(
        CalculateShippingCostRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationError = CalculateShippingCostValidator.Validate(request);
        if (validationError != null)
            return Task.FromResult(Result<CalculateShippingCostResponse>.Failure(validationError));

        var volume = request.Dimensions!.Length * request.Dimensions.Width * request.Dimensions.Height;
        var distance = SimulateDistance(request.OriginZipCode!, request.DestinationZipCode!);

        var baseCost = distance * CostPerMile;
        var weightSurcharge = Math.Floor(request.Weight / 0.5m) * WeightSurchargePerHalfKg;
        var volumeSurcharge = volume * VolumeSurchargePerCm3;

        var subtotal = baseCost + weightSurcharge + volumeSurcharge;
        var finalCost = request.IsExpress
            ? subtotal * (1 + ExpressSurchargeMultiplier)
            : subtotal;

        finalCost = Math.Round(finalCost, 2);
        baseCost = Math.Round(baseCost, 2);

        var response = new CalculateShippingCostResponse
        {
            Id = Guid.NewGuid(),
            OriginZipCode = request.OriginZipCode!,
            DestinationZipCode = request.DestinationZipCode!,
            Weight = request.Weight,
            Volume = volume,
            IsExpress = request.IsExpress,
            BaseCost = baseCost,
            FinalCost = finalCost,
            CalculatedAt = DateTime.UtcNow
        };

        return Task.FromResult(Result<CalculateShippingCostResponse>.Success(response));
    }

    /// <summary>
    /// Simulates a distance in miles (100–3100) deterministically from two ZIP codes.
    /// </summary>
    private static decimal SimulateDistance(string origin, string destination)
    {
        int originSum = origin.Where(char.IsDigit).Sum(c => c - '0');
        int destSum = destination.Where(char.IsDigit).Sum(c => c - '0');
        // Range 100–3100 miles
        int distance = 100 + (Math.Abs(originSum * 17 - destSum * 13) % 3000);
        return distance;
    }
}
