using System.Text.Json.Serialization;

namespace UnusualWhales.Client.Models;

/// <summary>
/// Implied volatility term structure data point for a single expiry.
/// Represents the average of the latest volatilities for the at-the-money
/// call and put contracts for a given expiry date.
/// </summary>
public sealed class IvTermStructureData
{
    /// <summary>The date this data was recorded.</summary>
    [JsonPropertyName("date")]
    public string Date { get; init; } = string.Empty;

    /// <summary>Days to expiration.</summary>
    [JsonPropertyName("dte")]
    public int Dte { get; init; }

    /// <summary>Option expiry date (YYYY-MM-DD).</summary>
    [JsonPropertyName("expiry")]
    public string Expiry { get; init; } = string.Empty;

    /// <summary>Absolute implied move in dollars.</summary>
    [JsonPropertyName("implied_move")]
    public string? ImpliedMove { get; init; }

    /// <summary>Implied move as a percentage of the underlying price.</summary>
    [JsonPropertyName("implied_move_perc")]
    public string? ImpliedMovePerc { get; init; }

    /// <summary>Ticker symbol.</summary>
    [JsonPropertyName("ticker")]
    public string Ticker { get; init; } = string.Empty;

    /// <summary>Average ATM implied volatility for this expiry.</summary>
    [JsonPropertyName("volatility")]
    public string? Volatility { get; init; }
}
