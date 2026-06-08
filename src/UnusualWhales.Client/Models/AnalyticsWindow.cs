using System.Text.Json;
using System.Text.Json.Serialization;

namespace UnusualWhales.Client.Models;

/// <summary>
/// Raw fixed-window analytics payload returned by
/// <c>GET /api/analytics/window</c>. The shape of <see cref="Meta"/> and
/// <see cref="Payload"/> depends on the requested <c>calculations</c>, so they
/// are surfaced as <see cref="JsonElement"/> for callers to navigate.
/// </summary>
public sealed class AnalyticsWindowData
{
    /// <summary>Metadata about the window (symbols, range, interval, etc.).</summary>
    [JsonPropertyName("meta")]
    public JsonElement Meta { get; init; }

    /// <summary>Calculation results keyed by calculation type (STDDEV, CORRELATION, etc.).</summary>
    [JsonPropertyName("payload")]
    public JsonElement Payload { get; init; }
}
