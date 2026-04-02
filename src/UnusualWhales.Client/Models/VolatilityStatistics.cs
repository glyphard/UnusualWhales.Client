using System.Text.Json.Serialization;

namespace UnusualWhales.Client.Models;

/// <summary>
/// Comprehensive volatility statistics for a ticker on a specific date, including
/// implied volatility data, realized volatility data, and their respective 52-week
/// high and low values.
/// </summary>
public sealed class VolatilityStatisticsData
{
    /// <summary>Trading date for this data point (YYYY-MM-DD).</summary>
    [JsonPropertyName("date")]
    public string Date { get; init; } = string.Empty;

    /// <summary>Ticker symbol.</summary>
    [JsonPropertyName("ticker")]
    public string Ticker { get; init; } = string.Empty;

    /// <summary>Current implied volatility.</summary>
    [JsonPropertyName("iv")]
    public string? ImpliedVolatility { get; init; }

    /// <summary>52-week low implied volatility.</summary>
    [JsonPropertyName("iv_low")]
    public string? IvLow { get; init; }

    /// <summary>52-week high implied volatility.</summary>
    [JsonPropertyName("iv_high")]
    public string? IvHigh { get; init; }

    /// <summary>IV rank (0–100) indicating where current IV stands within its 52-week range.</summary>
    [JsonPropertyName("iv_rank")]
    public string? IvRank { get; init; }

    /// <summary>Current realized (historical) volatility.</summary>
    [JsonPropertyName("rv")]
    public string? RealizedVolatility { get; init; }

    /// <summary>52-week low realized volatility.</summary>
    [JsonPropertyName("rv_low")]
    public string? RvLow { get; init; }

    /// <summary>52-week high realized volatility.</summary>
    [JsonPropertyName("rv_high")]
    public string? RvHigh { get; init; }
}
