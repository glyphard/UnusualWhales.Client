using System.Text.Json.Serialization;

namespace UnusualWhales.Client.Models;

/// <summary>
/// Realized and implied volatility data for a ticker on a given date.
/// The implied volatility is the expected 30-day forward-looking volatility.
/// The realized/historical volatility is the volatility of the stock price over the last 30 days.
/// </summary>
public sealed class RealizedVolatilityData
{
    /// <summary>Trading date for this data point (YYYY-MM-DD).</summary>
    [JsonPropertyName("date")]
    public string Date { get; init; } = string.Empty;

    /// <summary>Stock price on the given date.</summary>
    [JsonPropertyName("price")]
    public string? Price { get; init; }

    /// <summary>
    /// 30-day implied volatility (forward-looking).
    /// Shifted 30 days back relative to realized volatility for comparison purposes.
    /// </summary>
    [JsonPropertyName("implied_volatility")]
    public string? ImpliedVolatility { get; init; }

    /// <summary>30-day realized (historical) volatility.</summary>
    [JsonPropertyName("realized_volatility")]
    public string? RealizedVolatility { get; init; }

    /// <summary>
    /// The unshifted date used for the realized volatility calculation.
    /// This represents the actual date 30 days forward from the trading date.
    /// </summary>
    [JsonPropertyName("unshifted_rv_date")]
    public string? UnshiftedRvDate { get; init; }
}
