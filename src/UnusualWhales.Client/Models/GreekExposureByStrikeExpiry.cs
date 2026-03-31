using System.Text.Json.Serialization;

namespace UnusualWhales.Client.Models;

/// <summary>
/// Greek exposure for a ticker grouped by strike price for a specific expiry date.
/// </summary>
public sealed class GreekExposureByStrikeExpiryData
{
    /// <summary>Option expiry date (YYYY-MM-DD).</summary>
    [JsonPropertyName("expiry")]
    public string Expiry { get; init; } = string.Empty;

    /// <summary>The option strike price.</summary>
    [JsonPropertyName("strike")]
    public string Strike { get; init; } = string.Empty;

    /// <summary>Aggregate call charm exposure at this strike and expiry.</summary>
    [JsonPropertyName("call_charm")]
    public string CallCharm { get; init; } = string.Empty;

    /// <summary>Aggregate call delta exposure at this strike and expiry.</summary>
    [JsonPropertyName("call_delta")]
    public string CallDelta { get; init; } = string.Empty;

    /// <summary>Aggregate call gamma exposure (GEX) at this strike and expiry.</summary>
    [JsonPropertyName("call_gamma")]
    public string CallGamma { get; init; } = string.Empty;

    /// <summary>Aggregate call vanna exposure at this strike and expiry.</summary>
    [JsonPropertyName("call_vanna")]
    public string CallVanna { get; init; } = string.Empty;

    /// <summary>Aggregate put charm exposure at this strike and expiry.</summary>
    [JsonPropertyName("put_charm")]
    public string PutCharm { get; init; } = string.Empty;

    /// <summary>Aggregate put delta exposure at this strike and expiry.</summary>
    [JsonPropertyName("put_delta")]
    public string PutDelta { get; init; } = string.Empty;

    /// <summary>Aggregate put gamma exposure at this strike and expiry.</summary>
    [JsonPropertyName("put_gamma")]
    public string PutGamma { get; init; } = string.Empty;

    /// <summary>Aggregate put vanna exposure at this strike and expiry.</summary>
    [JsonPropertyName("put_vanna")]
    public string PutVanna { get; init; } = string.Empty;
}
