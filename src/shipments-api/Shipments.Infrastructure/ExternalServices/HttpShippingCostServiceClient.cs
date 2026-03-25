using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shipments.Application.ExternalServices;
using Shipments.Domain.Models;

namespace Shipments.Infrastructure.ExternalServices;

/// <summary>
/// HTTP implementation of IShippingCostServiceClient that calls the external Costs API.
/// Returns null when the Costs API is unavailable so the caller can return 503.
/// </summary>
public class HttpShippingCostServiceClient : IShippingCostServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpShippingCostServiceClient> _logger;

    public HttpShippingCostServiceClient(HttpClient httpClient, ILogger<HttpShippingCostServiceClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<decimal?> CalculateShippingCostAsync(
        string originZipCode,
        string destinationZipCode,
        decimal weight,
        Dimensions dimensions,
        CancellationToken cancellationToken = default)
    {
        var requestBody = new
        {
            OriginZipCode = originZipCode,
            DestinationZipCode = destinationZipCode,
            Weight = weight,
            Dimensions = new
            {
                dimensions.Length,
                dimensions.Width,
                dimensions.Height
            },
            IsExpress = false
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/v1/costs/calculate", requestBody, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Costs API returned non-success status {StatusCode} for shipping cost calculation.",
                    (int)response.StatusCode);
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<CostsApiResponse>(
                cancellationToken: cancellationToken);

            return result?.FinalCost;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reach the Costs API for shipping cost calculation.");
            return null;
        }
    }

    private sealed class CostsApiResponse
    {
        public decimal FinalCost { get; set; }
    }
}
