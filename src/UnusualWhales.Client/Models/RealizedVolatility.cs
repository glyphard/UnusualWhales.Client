using System.Text.Json.Serialization;

namespace UnusualWhales.Client.Models;

/// <summary>
/// Realized and implied volatility data for a ticker on a given date.
/// The implied volatility is the expected 30-day forward-looking volatility.
/// The realized/historical volatility is the volatility of the stock price over the last 30 days.
/// </summary>
public sealed class RealizedVolatilityData
{
    /// <summary>Trading date for this data point.</summary>
    [JsonPropertyName("date")]
    public string Date { get; init; } = string.Empty;

    /// <summary>
    /// 30-day implied volatility (forward-looking).
    /// Shifted 30 days back relative to realized volatility for comparison purposes.
    /// </summary>
    [JsonPropertyName("implied_volatility")]
    public string? ImpliedVolatility { get; init; }

    /// <summary>30-day realized (historical) volatility.</summary>
    [JsonPropertyName("realized_volatility")]
    public string? RealizedVolatility { get; init; }
}
