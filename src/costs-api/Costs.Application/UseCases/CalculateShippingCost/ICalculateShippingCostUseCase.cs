using System.Threading;
using System.Threading.Tasks;
using Costs.Application.DTOs;
using Costs.Application.Results;

namespace Costs.Application.UseCases.CalculateShippingCost;

/// <summary>
/// Use case for calculating shipping cost.
/// </summary>
public interface ICalculateShippingCostUseCase
{
    Task<Result<CalculateShippingCostResponse>> HandleAsync(
        CalculateShippingCostRequest request,
        CancellationToken cancellationToken = default);
}
