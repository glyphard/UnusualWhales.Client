using System.Text.Json.Serialization;

namespace UnusualWhales.Client.Models;

/// <summary>
/// Pairwise Pearson correlation between two tickers over a date range.
/// Returned by <c>GET /api/market/correlations</c>.
/// </summary>
/// <remarks>
/// The endpoint returns one row per ordered pair of input tickers, including
/// self-pairs (which have correlation 1.0).
/// </remarks>
public sealed class CorrelationData
{
    /// <summary>Pearson correlation coefficient between the two tickers (−1 to 1).</summary>
    [JsonPropertyName("correlation")]
    public double Correlation { get; init; }

    /// <summary>First ticker in the pair.</summary>
    [JsonPropertyName("fst")]
    public string First { get; init; } = string.Empty;

    /// <summary>Second ticker in the pair.</summary>
    [JsonPropertyName("snd")]
    public string Second { get; init; } = string.Empty;

    /// <summary>Earliest trading date included in the calculation (YYYY-MM-DD).</summary>
    [JsonPropertyName("min_date")]
    public string? MinDate { get; init; }

    /// <summary>Latest trading date included in the calculation (YYYY-MM-DD).</summary>
    [JsonPropertyName("max_date")]
    public string? MaxDate { get; init; }

    /// <summary>Count of trading days included in the calculation.</summary>
    [JsonPropertyName("rows")]
    public int Rows { get; init; }
}
