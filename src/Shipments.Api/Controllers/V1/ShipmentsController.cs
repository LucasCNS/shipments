using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shipments.Application.UseCases.CreateShipment;

namespace Shipments.Api.Controllers.V1;

/// <summary>
/// Controller for shipment operations.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/shipments")]
[Produces("application/json")]
public class ShipmentsController : ControllerBase
{
    /// <summary>
    /// Creates a new shipment.
    /// </summary>
    /// <param name="useCase">The create shipment use case.</param>
    /// <param name="input">The shipment data to create.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The created shipment with status 201 Created.</returns>
    /// <response code="201">Shipment created successfully.</response>
    /// <response code="400">Invalid input data or missing Creator header.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync(
        [FromServices] ICreateShipmentUseCase useCase,
        [FromBody] CreateShipmentInput input,
        CancellationToken cancellationToken)
    {
        // Extract Creator from header
        var creator = Request.Headers["Creator"].ToString();

        if (string.IsNullOrWhiteSpace(creator))
        {
            return Problem(
                detail: "The 'Creator' header is required.",
                statusCode: StatusCodes.Status400BadRequest,
                title: "Missing Creator Header");
        }

        // Assign Creator to input
        input.Creator = creator;

        // Execute use case
        var output = await useCase.HandleAsync(input, cancellationToken);

        // Check if there was a validation error
        if (output.Error != null)
        {
            return Problem(
                detail: output.Error.Message,
                statusCode: output.Error.CorrespondingStatusCode,
                title: output.Error.Code);
        }

        // Return 201 Created
        return StatusCode(StatusCodes.Status201Created, output);
    }
}
