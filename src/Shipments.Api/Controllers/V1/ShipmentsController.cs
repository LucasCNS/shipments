using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shipments.Application.UseCases.CreateShipment;
using Shipments.Application.UseCases.GetShipmentById;
using Shipments.Application.UseCases.ListShipments;
using Shipments.Application.UseCases.UpdateShipment;

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
    /// <param name="creator">Required header to identify the request creator.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The created shipment with status 201 Created.</returns>
    /// <response code="201">Shipment created successfully.</response>
    /// <response code="400">Invalid input data or missing Creator header.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync(
        [FromServices] ICreateShipmentUseCase useCase,
        [FromHeader][Required] string creator,
        [FromBody] CreateShipmentInput input,
        CancellationToken cancellationToken)
    {
        // Assign Creator to input
        input.Creator = creator;

        // Execute use case
        var output = await useCase.HandleAsync(input, cancellationToken);

        // Check if there was a validation error
        if (output.Error != null)
        {
            return HandleUseCaseError(output.Error);
        }

        // Return 201 Created
        return StatusCode(StatusCodes.Status201Created, output);
    }

    /// <summary>
    /// Lists shipments with optional filtering by status and pagination.
    /// </summary>
    /// <param name="useCase">The list shipments use case.</param>
    /// <param name="status">Optional status filter (pending, in_transit, delivered, cancelled).</param>
    /// <param name="limit">Number of records to return (default: 10, max: 100).</param>
    /// <param name="offset">Number of records to skip (default: 0).</param>
    /// <param name="creator">Required header to identify the request creator.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A paginated list of shipments with status 200 OK.</returns>
    /// <response code="200">Shipments retrieved successfully.</response>
    /// <response code="400">Invalid query parameters or missing Creator header.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListAsync(
        [FromServices] IListShipmentsUseCase useCase,
        [FromHeader][Required] string creator,
        [FromQuery] string? status,
        [FromQuery] int limit = 10,
        [FromQuery] int offset = 0,
        CancellationToken cancellationToken = default)
    {
        // Create input
        var input = new ListShipmentsInput
        {
            Status = status,
            Limit = limit,
            Offset = offset,
            Creator = creator
        };

        // Execute use case
        var output = await useCase.HandleAsync(input, cancellationToken);

        // Check if there was a validation error
        if (output.Error != null)
        {
            return HandleUseCaseError(output.Error);
        }

        // Return 200 OK
        return Ok(output);
    }

    /// <summary>
    /// Retrieves a shipment by its ID.
    /// </summary>
    /// <param name="useCase">The get shipment by ID use case.</param>
    /// <param name="id">The shipment ID to retrieve.</param>
    /// <param name="creator">Required header to identify the request creator.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The shipment details with status 200 OK.</returns>
    /// <response code="200">Shipment found and returned successfully.</response>
    /// <response code="400">Invalid shipment ID format or missing Creator header.</response>
    /// <response code="404">Shipment not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(
        [FromServices] IGetShipmentByIdUseCase useCase,
        [FromRoute] string id,
        [FromHeader][Required] string creator,
        CancellationToken cancellationToken)
    {
        // Create input
        var input = new GetShipmentByIdInput { ShipmentId = id };

        // Execute use case
        var output = await useCase.HandleAsync(input, cancellationToken);

        // Check if there was an error
        if (output.Error != null)
        {
            return HandleUseCaseError(output.Error);
        }

        // Return 200 OK
        return Ok(output);
    }

    /// <summary>
    /// Updates an existing shipment's data fields.
    /// Status updates are NOT supported through this endpoint. Only shipments with 'pending' status can have their data fields updated.
    /// </summary>
    /// <param name="useCase">The update shipment use case.</param>
    /// <param name="id">The shipment ID to update.</param>
    /// <param name="creator">Required header to identify the request creator.</param>
    /// <param name="input">The shipment data to update (PackageName, Weight, Dimensions, ShippingCost, DestinationAddress).</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The updated shipment with status 200 OK.</returns>
    /// <response code="200">Shipment updated successfully.</response>
    /// <response code="400">Invalid input data, missing Creator header, or no fields provided for update.</response>
    /// <response code="404">Shipment not found.</response>
    /// <response code="409">Shipment cannot be updated (e.g., not in pending status).</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAsync(
        [FromServices] IUpdateShipmentUseCase useCase,
        [FromRoute] string id,
        [FromHeader][Required] string creator,
        [FromBody] UpdateShipmentInput input,
        CancellationToken cancellationToken)
    {
        // Assign ID and Creator to input
        input.Id = id;
        input.Creator = creator;

        // Execute use case
        var output = await useCase.HandleAsync(input, cancellationToken);

        // Check if there was an error
        if (output.Error != null)
        {
            return HandleUseCaseError(output.Error);
        }

        // Return 200 OK
        return Ok(output);
    }

    /// <summary>
    /// Handles use case errors by returning a Problem response.
    /// </summary>
    /// <param name="error">The error from the use case execution.</param>
    /// <returns>A Problem response with appropriate status code.</returns>
    private IActionResult HandleUseCaseError(Domain.Results.Error error)
    {
        // If there are multiple validation errors, return them all
        if (error.ValidationErrors.Count > 0)
        {
            return new JsonResult(
                new
                {
                    type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    title = error.Code,
                    status = error.CorrespondingStatusCode,
                    detail = error.Message,
                    errors = error.ValidationErrors
                })
            {
                StatusCode = error.CorrespondingStatusCode
            };
        }

        return Problem(
            detail: error.Message,
            statusCode: error.CorrespondingStatusCode,
            title: error.Code);
    }
}
