using UnusualWhales.Client.Clients;

namespace UnusualWhales.Client;

/// <summary>
/// The top-level entry point for the Unusual Whales API client.
/// </summary>
/// <example>
/// <code>
/// var client = new UnusualWhalesClient(new UnusualWhalesClientOptions
/// {
///     ApiKey = "YOUR_API_KEY",
/// });
///
/// var greekExposure = await client.Stocks.GetGreekExposureAsync("AAPL");
/// </code>
/// </example>
public sealed class UnusualWhalesClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly bool _ownsHttpClient;
    private bool _disposed;

    /// <summary>
    /// Provides access to the Stocks API endpoints, including greek exposure,
    /// volatility, and technical indicators.
    /// </summary>
    public StocksClient Stocks { get; }

    /// <summary>
    /// Initialises a new client using the provided options.
    /// A new <see cref="HttpClient"/> is created and owned by this instance.
    /// </summary>
    /// <param name="options">Configuration including API key and optional base URL.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <see cref="UnusualWhalesClientOptions.ApiKey"/> is null or whitespace.</exception>
    public UnusualWhalesClient(UnusualWhalesClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.ApiKey, nameof(options.ApiKey));

        _httpClient = CreateHttpClient(options);
        _ownsHttpClient = true;
        Stocks = new StocksClient(_httpClient);
    }

    /// <summary>
    /// Initialises a new client using an externally managed <see cref="HttpClient"/>.
    /// Use this overload when you want full control over the HTTP pipeline
    /// (e.g. from <c>IHttpClientFactory</c>).
    /// The supplied <paramref name="httpClient"/> is <strong>not</strong> disposed when this
    /// <see cref="UnusualWhalesClient"/> is disposed.
    /// </summary>
    /// <param name="httpClient">
    /// A pre-configured <see cref="HttpClient"/> whose <see cref="HttpClient.BaseAddress"/>
    /// and authorization header are already set.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="httpClient"/> is null.</exception>
    public UnusualWhalesClient(HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        _httpClient = httpClient;
        _ownsHttpClient = false;
        Stocks = new StocksClient(_httpClient);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!_disposed)
        {
            if (_ownsHttpClient)
                _httpClient.Dispose();

            _disposed = true;
        }
    }

    // ── Private helpers ──────────────────────────────────────────────────────────

    private static HttpClient CreateHttpClient(UnusualWhalesClientOptions options)
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri(options.BaseUrl),
        };

        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {options.ApiKey}");
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        return client;
    }
}
