using System.Text.Json.Serialization;

namespace UnusualWhales.Client.Models;

/// <summary>
/// Technical indicators for a ticker on a given date, including momentum,
/// trend, and volatility-based signals.
/// </summary>
public sealed class TechnicalIndicatorsData
{
    /// <summary>Trading date for this data point.</summary>
    [JsonPropertyName("date")]
    public string Date { get; init; } = string.Empty;

    /// <summary>Ticker symbol.</summary>
    [JsonPropertyName("ticker")]
    public string Ticker { get; init; } = string.Empty;

    /// <summary>Relative Strength Index (14-period).</summary>
    [JsonPropertyName("rsi")]
    public string? Rsi { get; init; }

    /// <summary>20-period simple moving average.</summary>
    [JsonPropertyName("sma_20")]
    public string? Sma20 { get; init; }

    /// <summary>50-period simple moving average.</summary>
    [JsonPropertyName("sma_50")]
    public string? Sma50 { get; init; }

    /// <summary>200-period simple moving average.</summary>
    [JsonPropertyName("sma_200")]
    public string? Sma200 { get; init; }

    /// <summary>20-period exponential moving average.</summary>
    [JsonPropertyName("ema_20")]
    public string? Ema20 { get; init; }

    /// <summary>50-period exponential moving average.</summary>
    [JsonPropertyName("ema_50")]
    public string? Ema50 { get; init; }

    /// <summary>MACD line value.</summary>
    [JsonPropertyName("macd")]
    public string? Macd { get; init; }

    /// <summary>MACD signal line value.</summary>
    [JsonPropertyName("macd_signal")]
    public string? MacdSignal { get; init; }

    /// <summary>MACD histogram value.</summary>
    [JsonPropertyName("macd_histogram")]
    public string? MacdHistogram { get; init; }

    /// <summary>Upper Bollinger Band.</summary>
    [JsonPropertyName("bb_upper")]
    public string? BollingerUpper { get; init; }

    /// <summary>Middle Bollinger Band (20-period SMA).</summary>
    [JsonPropertyName("bb_middle")]
    public string? BollingerMiddle { get; init; }

    /// <summary>Lower Bollinger Band.</summary>
    [JsonPropertyName("bb_lower")]
    public string? BollingerLower { get; init; }
}
