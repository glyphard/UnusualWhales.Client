using System.Text.Json.Serialization;

namespace UnusualWhales.Client.Models;

/// <summary>
/// Daily aggregate greek exposure for a ticker.
/// Represents the assumed greek exposure that market makers are exposed to for all open contracts
/// on a given trading date.
/// </summary>
public sealed class GreekExposureData
{
    /// <summary>Trading date for this data point.</summary>
    [JsonPropertyName("date")]
    public string Date { get; init; } = string.Empty;

    /// <summary>Aggregate call charm exposure.</summary>
    [JsonPropertyName("call_charm")]
    public string CallCharm { get; init; } = string.Empty;

    /// <summary>Aggregate call delta exposure.</summary>
    [JsonPropertyName("call_delta")]
    public string CallDelta { get; init; } = string.Empty;

    /// <summary>Aggregate call gamma exposure (GEX).</summary>
    [JsonPropertyName("call_gamma")]
    public string CallGamma { get; init; } = string.Empty;

    /// <summary>Aggregate call vanna exposure.</summary>
    [JsonPropertyName("call_vanna")]
    public string CallVanna { get; init; } = string.Empty;

    /// <summary>Aggregate put charm exposure.</summary>
    [JsonPropertyName("put_charm")]
    public string PutCharm { get; init; } = string.Empty;

    /// <summary>Aggregate put delta exposure (DEX).</summary>
    [JsonPropertyName("put_delta")]
    public string PutDelta { get; init; } = string.Empty;

    /// <summary>Aggregate put gamma exposure.</summary>
    [JsonPropertyName("put_gamma")]
    public string PutGamma { get; init; } = string.Empty;

    /// <summary>Aggregate put vanna exposure.</summary>
    [JsonPropertyName("put_vanna")]
    public string PutVanna { get; init; } = string.Empty;
}
