using System.Text.Json.Serialization;

namespace UnusualWhales.Client.Models;

/// <summary>
/// Greek exposure for a ticker grouped by expiry date across all contracts on a given market date.
/// </summary>
public sealed class GreekExposureByExpiryData
{
    /// <summary>Option expiry date (YYYY-MM-DD).</summary>
    [JsonPropertyName("expiry")]
    public string Expiry { get; init; } = string.Empty;

    /// <summary>Aggregate call charm exposure for this expiry.</summary>
    [JsonPropertyName("call_charm")]
    public string CallCharm { get; init; } = string.Empty;

    /// <summary>Aggregate call delta exposure for this expiry.</summary>
    [JsonPropertyName("call_delta")]
    public string CallDelta { get; init; } = string.Empty;

    /// <summary>Aggregate call gamma exposure (GEX) for this expiry.</summary>
    [JsonPropertyName("call_gamma")]
    public string CallGamma { get; init; } = string.Empty;

    /// <summary>Aggregate call vanna exposure for this expiry.</summary>
    [JsonPropertyName("call_vanna")]
    public string CallVanna { get; init; } = string.Empty;

    /// <summary>Aggregate put charm exposure for this expiry.</summary>
    [JsonPropertyName("put_charm")]
    public string PutCharm { get; init; } = string.Empty;

    /// <summary>Aggregate put delta exposure for this expiry.</summary>
    [JsonPropertyName("put_delta")]
    public string PutDelta { get; init; } = string.Empty;

    /// <summary>Aggregate put gamma exposure for this expiry.</summary>
    [JsonPropertyName("put_gamma")]
    public string PutGamma { get; init; } = string.Empty;

    /// <summary>Aggregate put vanna exposure for this expiry.</summary>
    [JsonPropertyName("put_vanna")]
    public string PutVanna { get; init; } = string.Empty;
}
