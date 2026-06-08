using System.Net.Http.Json;
using System.Text.Json;
using UnusualWhales.Client.Models;

namespace UnusualWhales.Client.Clients;

/// <summary>
/// Provides access to market-wide and basket-of-tickers endpoints on the
/// Unusual Whales API, such as cross-sectional correlations and fixed-window
/// analytics (volatility, covariance, drawdown, etc.).
/// </summary>
public sealed class MarketClient
{
    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    internal MarketClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Returns pairwise Pearson correlations between a basket of tickers over a
    /// date range.
    /// </summary>
    /// <remarks>
    /// Path: <c>GET /api/market/correlations</c><br/>
    /// Operation ID: <c>PublicApi.MarketController.correlations</c><br/>
    /// <br/>
    /// If both <paramref name="interval"/> and <paramref name="startDate"/> are
    /// supplied, the server gives <paramref name="interval"/> priority.
    /// </remarks>
    /// <param name="tickers">Basket of tickers to correlate (e.g. <c>["AAPL", "SPY"]</c>).</param>
    /// <param name="interval">
    /// Optional timeframe shorthand (e.g. "1Y", "6M", "3M", "1M").
    /// </param>
    /// <param name="startDate">Optional ISO date (YYYY-MM-DD) to start the window.</param>
    /// <param name="endDate">Optional ISO date (YYYY-MM-DD) to end the window.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>One <see cref="CorrelationData"/> per ordered pair of tickers.</returns>
    public async Task<IReadOnlyList<CorrelationData>> GetCorrelationsAsync(
        IEnumerable<string> tickers,
        string? interval = null,
        string? startDate = null,
        string? endDate = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(tickers);

        var tickerCsv = string.Join(",", tickers);
        if (string.IsNullOrWhiteSpace(tickerCsv))
            throw new ArgumentException("At least one ticker must be supplied.", nameof(tickers));

        var url = BuildUrl("/api/market/correlations",
            ("tickers", tickerCsv),
            ("interval", interval),
            ("start_date", startDate),
            ("end_date", endDate));

        var response = await _httpClient
            .GetFromJsonAsync<ApiResponse<IReadOnlyList<CorrelationData>>>(url, JsonOptions, cancellationToken)
            .ConfigureAwait(false);

        return response?.Data ?? [];
    }

    /// <summary>
    /// Returns fixed-window statistical analytics for a basket of tickers — including
    /// volatility (STDDEV), correlation matrix, covariance, drawdown, autocorrelation,
    /// and more.
    /// </summary>
    /// <remarks>
    /// Path: <c>GET /api/analytics/window</c><br/>
    /// Operation ID: <c>PublicApi.IntelController.analytics_window</c><br/>
    /// <br/>
    /// Requires the Advanced+ subscription tier on Unusual Whales.
    /// </remarks>
    /// <param name="symbols">Basket of tickers (e.g. <c>["AAPL", "IBM", "MSFT"]</c>).</param>
    /// <param name="range">
    /// Either an ISO start date (paired with <paramref name="rangeEnd"/>) or a
    /// relative shorthand like "2month" or "full".
    /// </param>
    /// <param name="calculations">
    /// Statistics to compute. Supported values: MIN, MAX, MEAN, MEDIAN,
    /// CUMULATIVE_RETURN, VARIANCE, STDDEV, MAX_DRAWDOWN, HISTOGRAM,
    /// AUTOCORRELATION, COVARIANCE, CORRELATION.
    /// </param>
    /// <param name="rangeEnd">
    /// Optional ISO end date, valid when <paramref name="range"/> is an ISO date.
    /// </param>
    /// <param name="interval">
    /// Optional candle interval: 1min, 5min, 15min, 30min, 60min, DAILY, WEEKLY,
    /// MONTHLY. Defaults to DAILY server-side.
    /// </param>
    /// <param name="ohlc">
    /// Optional OHLC field to use (open, high, low, close). Defaults to close
    /// server-side.
    /// </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// The raw analytics payload, or <see langword="null"/> if the API returned
    /// no data. The <see cref="AnalyticsWindowData.Payload"/> shape depends on
    /// the requested calculations.
    /// </returns>
    public async Task<AnalyticsWindowData?> GetAnalyticsWindowAsync(
        IEnumerable<string> symbols,
        string range,
        IEnumerable<string> calculations,
        string? rangeEnd = null,
        string? interval = null,
        string? ohlc = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(symbols);
        ArgumentNullException.ThrowIfNull(calculations);
        ArgumentException.ThrowIfNullOrWhiteSpace(range);

        var symbolsCsv = string.Join(",", symbols);
        if (string.IsNullOrWhiteSpace(symbolsCsv))
            throw new ArgumentException("At least one symbol must be supplied.", nameof(symbols));

        var calcsCsv = string.Join(",", calculations);
        if (string.IsNullOrWhiteSpace(calcsCsv))
            throw new ArgumentException("At least one calculation must be supplied.", nameof(calculations));

        var url = BuildUrl("/api/analytics/window",
            ("symbols", symbolsCsv),
            ("range", range),
            ("range_end", rangeEnd),
            ("interval", interval),
            ("ohlc", ohlc),
            ("calculations", calcsCsv));

        var response = await _httpClient
            .GetFromJsonAsync<ApiResponse<AnalyticsWindowData>>(url, JsonOptions, cancellationToken)
            .ConfigureAwait(false);

        return response?.Data;
    }

    // ── Helpers ─────────────────────────────────────────────────────────────────

    private static string BuildUrl(string path, params (string Key, string? Value)[] queryParams)
    {
        var pairs = queryParams.Where(p => p.Value is not null);

        if (!pairs.Any())
            return path;

        var query = string.Join("&",
            pairs.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value!)}"));

        return $"{path}?{query}";
    }
}
