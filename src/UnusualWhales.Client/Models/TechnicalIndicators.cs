using System.Text.Json.Serialization;

namespace UnusualWhales.Client.Models;

/// <summary>
/// Technical indicator data for a specific ticker, date, and indicator function.
/// The Values property contains the calculated indicator values, which vary by indicator type.
/// </summary>
public sealed class TechnicalIndicatorsData
{
    /// <summary>Trading date for this data point (YYYY-MM-DD).</summary>
    [JsonPropertyName("date")]
    public string Date { get; init; } = string.Empty;

    /// <summary>
    /// Dictionary containing the indicator values. The keys depend on the indicator function.
    /// For example, EMA/SMA/RSI return a single key matching the function name.
    /// MACD returns "MACD", "MACD_Hist", and "MACD_Signal".
    /// BBANDS returns "Real Upper Band", "Real Middle Band", and "Real Lower Band".
    /// </summary>
    [JsonPropertyName("values")]
    public Dictionary<string, string> Values { get; init; } = new();

    /// <summary>Ticker symbol.</summary>
    [JsonPropertyName("ticker")]
    public string Ticker { get; init; } = string.Empty;

    /// <summary>The interval used (e.g., "daily", "weekly", "1min", "5min").</summary>
    [JsonPropertyName("interval")]
    public string Interval { get; init; } = string.Empty;

    /// <summary>The technical indicator function (e.g., "EMA", "SMA", "RSI", "MACD").</summary>
    [JsonPropertyName("indicator")]
    public string Indicator { get; init; } = string.Empty;

    /// <summary>The series type used for calculation (e.g., "close", "open", "high", "low").</summary>
    [JsonPropertyName("series_type")]
    public string? SeriesType { get; init; }

    /// <summary>The time period used for the indicator calculation.</summary>
    [JsonPropertyName("time_period")]
    public int? TimePeriod { get; init; }

    /// <summary>
    /// Helper method to get a single indicator value. Useful when the indicator
    /// returns only one value (like EMA, SMA, RSI).
    /// </summary>
    /// <returns>The first value in the Values dictionary, or null if empty.</returns>
    public string? GetSingleValue()
    {
        return Values.Values.FirstOrDefault();
    }

    /// <summary>
    /// Helper method to try get a specific value from the Values dictionary.
    /// </summary>
    /// <param name="key">The key to look up (e.g., "MACD", "RSI", "Real Upper Band").</param>
    /// <returns>The value if found, otherwise null.</returns>
    public string? GetValue(string key)
    {
        return Values.TryGetValue(key, out var value) ? value : null;
    }
}
