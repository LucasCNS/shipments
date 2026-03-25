using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using Costs.Application.DTOs;
using Costs.Application.UseCases.CalculateShippingCost;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Costs.Api.Controllers.V1;

/// <summary>
/// Controller for shipping cost operations.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/costs")]
[Produces("application/json")]
public class CostsController : ControllerBase
{
    /// <summary>
    /// Calculates the shipping cost for a given package.
    /// </summary>
    /// <param name="useCase">The calculate shipping cost use case.</param>
    /// <param name="request">The shipping cost calculation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Calculated shipping cost details.</returns>
    /// <response code="200">Shipping cost calculated successfully.</response>
    /// <response code="400">Invalid input data.</response>
    [HttpPost("calculate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CalculateAsync(
        [FromServices] ICalculateShippingCostUseCase useCase,
        [FromBody] CalculateShippingCostRequest request,
        CancellationToken cancellationToken)
    {
        var result = await useCase.HandleAsync(request, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                result.Error!.Code,
                result.Error.Message,
                result.Error.ValidationErrors
            });
        }

        return Ok(result.Value);
    }
}
