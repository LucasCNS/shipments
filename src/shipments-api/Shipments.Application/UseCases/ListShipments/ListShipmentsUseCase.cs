using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shipments.Application.Repositories;
using Shipments.Application.Validators;

namespace Shipments.Application.UseCases.ListShipments;

/// <summary>
/// Implementation of the ListShipments use case.
/// </summary>
public class ListShipmentsUseCase : IListShipmentsUseCase
{
    private readonly IShipmentRepository _repository;
    private readonly ILogger<ListShipmentsUseCase> _logger;

    /// <summary>
    /// Initializes a new instance of the ListShipmentsUseCase class.
    /// </summary>
    /// <param name="repository">The shipment repository.</param>
    /// <param name="logger">The logger.</param>
    public ListShipmentsUseCase(IShipmentRepository repository, ILogger<ListShipmentsUseCase> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the retrieval of a list of shipments with optional filtering and pagination.
    /// </summary>
    /// <param name="input">The input data for listing shipments.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The shipments list output.</returns>
    public async Task<ListShipmentsOutput> HandleAsync(ListShipmentsInput input, CancellationToken cancellationToken)
    {
        // Validate input
        var validationError = ListShipmentsValidator.Validate(input);
        if (validationError != null)
        {
            _logger.LogWarning("Validation error when listing shipments: {Code} - {Message}",
                validationError.Code, validationError.Message);

            return new ListShipmentsOutput
            {
                Error = validationError
            };
        }

        // Log the request
        _logger.LogInformation("Solicitação de listagem: status={Status}, limit={Limit}, offset={Offset}",
            input.Status ?? "null", input.Limit, input.Offset);

        // Get total count
        var total = await _repository.GetCountAsync(input.Status, cancellationToken);

        // Get paginated results
        var results = await _repository.GetAllAsync(input.Status, input.Offset, input.Limit, cancellationToken);

        _logger.LogInformation("Total={Total}, retornando {Count} a partir de {Offset}",
            total, results.Count(), input.Offset);

        // Return output
        return new ListShipmentsOutput
        {
            Total = total,
            Offset = input.Offset,
            Limit = input.Limit,
            Results = results.ToList(),
            Error = null
        };
    }
}
