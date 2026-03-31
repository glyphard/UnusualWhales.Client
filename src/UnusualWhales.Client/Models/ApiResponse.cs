using System.Text.Json.Serialization;

namespace UnusualWhales.Client.Models;

/// <summary>
/// Wraps a paginated or single-item API response whose JSON looks like <c>{ "data": … }</c>.
/// </summary>
/// <typeparam name="T">The type of the <c>data</c> payload.</typeparam>
public sealed class ApiResponse<T>
{
    /// <summary>The deserialized payload returned in the <c>data</c> field.</summary>
    [JsonPropertyName("data")]
    public T Data { get; init; } = default!;
}
