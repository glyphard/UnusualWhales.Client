namespace UnusualWhales.Client;

/// <summary>
/// Configuration options for <see cref="UnusualWhalesClient"/>.
/// </summary>
public sealed class UnusualWhalesClientOptions
{
    /// <summary>
    /// The Unusual Whales API bearer token.
    /// Obtain one from <see href="https://unusualwhales.com"/>.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// The base URL of the Unusual Whales API.
    /// Defaults to <c>https://api.unusualwhales.com</c>.
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.unusualwhales.com";
}
