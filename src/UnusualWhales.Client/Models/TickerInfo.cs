using System.Text.Json.Serialization;

namespace UnusualWhales.Client.Models;

/// <summary>
/// General information about a ticker, returned by
/// <c>GET /api/stock/{ticker}/info</c>.
/// Includes the beta coefficient measured against the S&amp;P 500 (SPX).
/// </summary>
public sealed class TickerInfoData
{
    /// <summary>The ticker symbol (e.g. "AAPL").</summary>
    [JsonPropertyName("symbol")]
    public string Symbol { get; init; } = string.Empty;

    /// <summary>Full company name.</summary>
    [JsonPropertyName("full_name")]
    public string? FullName { get; init; }

    /// <summary>
    /// Beta coefficient measuring the stock's volatility relative to the S&amp;P 500.
    /// A value of 1 indicates the stock moves with the market; greater than 1
    /// indicates higher volatility, less than 1 indicates lower volatility.
    /// </summary>
    [JsonPropertyName("beta")]
    public string? Beta { get; init; }

    /// <summary>Market sector (e.g. "Technology").</summary>
    [JsonPropertyName("sector")]
    public string? Sector { get; init; }

    /// <summary>Market capitalisation in USD.</summary>
    [JsonPropertyName("marketcap")]
    public string? MarketCap { get; init; }

    /// <summary>Market cap bucket (e.g. "big", "mid", "small").</summary>
    [JsonPropertyName("marketcap_size")]
    public string? MarketCapSize { get; init; }

    /// <summary>Average daily share volume over the last 30 days.</summary>
    [JsonPropertyName("avg30_volume")]
    public string? Avg30Volume { get; init; }

    /// <summary>Short company description.</summary>
    [JsonPropertyName("short_description")]
    public string? ShortDescription { get; init; }

    /// <summary>Issue type (e.g. "Common Stock").</summary>
    [JsonPropertyName("issue_type")]
    public string? IssueType { get; init; }

    /// <summary>True if the company pays a dividend.</summary>
    [JsonPropertyName("has_dividend")]
    public bool? HasDividend { get; init; }

    /// <summary>True if earnings history is available.</summary>
    [JsonPropertyName("has_earnings_history")]
    public bool? HasEarningsHistory { get; init; }

    /// <summary>True if the company has an investment arm.</summary>
    [JsonPropertyName("has_investment_arm")]
    public bool? HasInvestmentArm { get; init; }

    /// <summary>True if listed options exist for this ticker.</summary>
    [JsonPropertyName("has_options")]
    public bool? HasOptions { get; init; }

    /// <summary>When earnings are announced (e.g. "premarket", "postmarket", "unknown").</summary>
    [JsonPropertyName("announce_time")]
    public string? AnnounceTime { get; init; }

    /// <summary>Next scheduled earnings date (YYYY-MM-DD), if known.</summary>
    [JsonPropertyName("next_earnings_date")]
    public string? NextEarningsDate { get; init; }

    /// <summary>Unusual Whales tags associated with this ticker.</summary>
    [JsonPropertyName("uw_tags")]
    public IReadOnlyList<string>? UwTags { get; init; }
}
