using Swashbuckle.AspNetCore.Annotations;

namespace ExchangeRateApi.Models;

/// <summary>
/// Response model for exchange rates
/// </summary>
[SwaggerSchema("Response containing exchange rates and metadata")]
public class ExchangeRateResponse
{
	/// <summary>
	/// Date the rate is valid for (UTC) if supplied by upstream provider
	/// </summary>
	/// <example>2025-01-02T00:00:00Z</example>
	[SwaggerSchema("Date (UTC) the upstream provider states the rate is valid for; null if not provided")]
	public DateTime? ValidFor { get; set; }

	/// <summary>
	/// The target currency used for the exchange rates
	/// </summary>
	/// <example>CZK</example>
	[SwaggerSchema("Target currency code used for all exchange rates")]
    public string TargetCurrency { get; set; } = string.Empty;

	/// <summary>
	/// List of exchange rates
	/// </summary>
	[SwaggerSchema("Array of exchange rate objects")]
    public List<ExchangeRateDto> Rates { get; set; } = new();
}