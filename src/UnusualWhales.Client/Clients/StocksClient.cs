using System.Net.Http.Json;
using System.Text.Json;
using UnusualWhales.Client.Models;

namespace UnusualWhales.Client.Clients;

/// <summary>
/// Provides access to the Stocks section of the Unusual Whales API,
/// covering greek exposure, volatility, and technical indicator endpoints.
/// </summary>
public sealed class StocksClient
{
    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    internal StocksClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Returns the daily aggregate greek exposure (GEX, DEX, etc.) for a ticker.
    /// </summary>
    /// <remarks>
    /// Path: <c>GET /api/stock/{ticker}/greek-exposure</c><br/>
    /// Operation ID: <c>PublicApi.TickerController.greek_exposure</c>
    /// </remarks>
    /// <param name="ticker">The stock ticker symbol (e.g. "AAPL").</param>
    /// <param name="date">
    /// Optional trading date filter (YYYY-MM-DD). Defaults to the last trading date.
    /// </param>
    /// <param name="timeframe">
    /// Optional timeframe for historical data (e.g. "1Y", "6M", "YTD").
    /// </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of daily greek exposure data points.</returns>
    public async Task<IReadOnlyList<GreekExposureData>> GetGreekExposureAsync(
        string ticker,
        string? date = null,
        string? timeframe = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ticker);

        var url = BuildUrl($"/api/stock/{Uri.EscapeDataString(ticker)}/greek-exposure",
            ("date", date), ("timeframe", timeframe));

        var response = await _httpClient
            .GetFromJsonAsync<ApiResponse<IReadOnlyList<GreekExposureData>>>(url, JsonOptions, cancellationToken)
            .ConfigureAwait(false);

        return response?.Data ?? [];
    }

    /// <summary>
    /// Returns the greek exposure for a ticker grouped by strike price for a given market date.
    /// </summary>
    /// <remarks>
    /// Path: <c>GET /api/stock/{ticker}/greek-exposure/strike</c><br/>
    /// Operation ID: <c>PublicApi.TickerController.greek_exposure_by_strike</c>
    /// </remarks>
    /// <param name="ticker">The stock ticker symbol.</param>
    /// <param name="date">Optional trading date filter (YYYY-MM-DD).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of greek exposure data points, one per strike price.</returns>
    public async Task<IReadOnlyList<GreekExposureByStrikeData>> GetGreekExposureByStrikeAsync(
        string ticker,
        string? date = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ticker);

        var url = BuildUrl($"/api/stock/{Uri.EscapeDataString(ticker)}/greek-exposure/strike",
            ("date", date));

        var response = await _httpClient
            .GetFromJsonAsync<ApiResponse<IReadOnlyList<GreekExposureByStrikeData>>>(url, JsonOptions, cancellationToken)
            .ConfigureAwait(false);

        return response?.Data ?? [];
    }

    /// <summary>
    /// Returns the greek exposure for a ticker grouped by expiry date across all contracts
    /// on a given market date.
    /// </summary>
    /// <remarks>
    /// Path: <c>GET /api/stock/{ticker}/greek-exposure/expiry</c><br/>
    /// Operation ID: <c>PublicApi.TickerController.greek_exposure_by_expiry</c>
    /// </remarks>
    /// <param name="ticker">The stock ticker symbol.</param>
    /// <param name="date">Optional trading date filter (YYYY-MM-DD).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of greek exposure data points, one per expiry date.</returns>
    public async Task<IReadOnlyList<GreekExposureByExpiryData>> GetGreekExposureByExpiryAsync(
        string ticker,
        string? date = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ticker);

        var url = BuildUrl($"/api/stock/{Uri.EscapeDataString(ticker)}/greek-exposure/expiry",
            ("date", date));

        var response = await _httpClient
            .GetFromJsonAsync<ApiResponse<IReadOnlyList<GreekExposureByExpiryData>>>(url, JsonOptions, cancellationToken)
            .ConfigureAwait(false);

        return response?.Data ?? [];
    }

    /// <summary>
    /// Returns the greek exposure for a ticker grouped by strike price for a specific expiry date.
    /// </summary>
    /// <remarks>
    /// Path: <c>GET /api/stock/{ticker}/greek-exposure/strike-expiry</c><br/>
    /// Operation ID: <c>PublicApi.TickerController.greek_exposure_by_strike_expiry</c>
    /// </remarks>
    /// <param name="ticker">The stock ticker symbol.</param>
    /// <param name="expiry">
    /// The expiry date to filter by (YYYY-MM-DD). Required by the API.
    /// </param>
    /// <param name="date">Optional trading date filter (YYYY-MM-DD).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of greek exposure data points per strike for the given expiry.</returns>
    public async Task<IReadOnlyList<GreekExposureByStrikeExpiryData>> GetGreekExposureByStrikeExpiryAsync(
        string ticker,
        string expiry,
        string? date = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ticker);
        ArgumentException.ThrowIfNullOrWhiteSpace(expiry);

        var url = BuildUrl($"/api/stock/{Uri.EscapeDataString(ticker)}/greek-exposure/strike-expiry",
            ("expiry", expiry), ("date", date));

        var response = await _httpClient
            .GetFromJsonAsync<ApiResponse<IReadOnlyList<GreekExposureByStrikeExpiryData>>>(url, JsonOptions, cancellationToken)
            .ConfigureAwait(false);

        return response?.Data ?? [];
    }

    /// <summary>
    /// Returns technical indicator time series for a ticker. Supports international
    /// stocks and OTC.
    /// </summary>
    /// <remarks>
    /// Path: <c>GET /api/stock/{ticker}/technical-indicator/{function}</c><br/>
    /// Operation ID: <c>PublicApi.AvFundamentalController.technical_indicator</c><br/>
    /// <br/>
    /// Supported functions: SMA, EMA, WMA, DEMA, TEMA, TRIMA, KAMA, MAMA, T3, 
    /// MACD, MACDEXT, STOCH, STOCHF, RSI, STOCHRSI, WILLR, ADX, ADXR, APO, PPO, 
    /// MOM, BOP, CCI, CMO, ROC, ROCR, AROON, AROONOSC, MFI, TRIX, ULTOSC, DX, 
    /// MINUS_DI, PLUS_DI, MINUS_DM, PLUS_DM, BBANDS, MIDPOINT, MIDPRICE, SAR, 
    /// TRANGE, ATR, NATR, AD, ADOSC, OBV, HT_TRENDLINE, HT_SINE, HT_TRENDMODE, 
    /// HT_DCPERIOD, HT_DCPHASE, HT_PHASOR, VWAP.
    /// </remarks>
    /// <param name="ticker">The stock ticker symbol.</param>
    /// <param name="function">The technical indicator function (e.g., "SMA", "RSI", "MACD").</param>
    /// <param name="interval">Optional interval (e.g., "daily", "weekly", "1min", "5min").</param>
    /// <param name="timePeriod">Optional time period for the indicator calculation.</param>
    /// <param name="seriesType">Optional series type (e.g., "close", "open", "high", "low").</param>
    /// <param name="month">Optional month filter (YYYY-MM), relevant for intraday intervals only.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of technical indicator data points.</returns>
    public async Task<IReadOnlyList<TechnicalIndicatorsData>> GetTechnicalIndicatorAsync(
        string ticker,
        string function,
        string? interval = null,
        int? timePeriod = null,
        string? seriesType = null,
        string? month = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ticker);
        ArgumentException.ThrowIfNullOrWhiteSpace(function);

        var url = BuildUrl($"/api/stock/{Uri.EscapeDataString(ticker)}/technical-indicator/{Uri.EscapeDataString(function)}",
            ("interval", interval),
            ("time_period", timePeriod?.ToString()),
            ("series_type", seriesType),
            ("month", month));

        var response = await _httpClient
            .GetFromJsonAsync<ApiResponse<IReadOnlyList<TechnicalIndicatorsData>>>(url, JsonOptions, cancellationToken)
            .ConfigureAwait(false);

        return response?.Data ?? [];
    }

    /// <summary>
    /// Returns the implied and realized volatility of a ticker over time.
    /// The implied volatility represents the expected 30-day forward-looking volatility;
    /// the realized volatility is shifted 30 days back for comparison.
    /// </summary>
    /// <remarks>
    /// Path: <c>GET /api/stock/{ticker}/volatility/realized</c><br/>
    /// Operation ID: <c>PublicApi.TickerController.realized_volatility</c>
    /// </remarks>
    /// <param name="ticker">The stock ticker symbol.</param>
    /// <param name="date">Optional trading date filter (YYYY-MM-DD).</param>
    /// <param name="timeframe">
    /// Optional timeframe for historical data (e.g. "1Y", "6M", "YTD").
    /// </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of realized/implied volatility data points.</returns>
    public async Task<IReadOnlyList<RealizedVolatilityData>> GetRealizedVolatilityAsync(
        string ticker,
        string? date = null,
        string? timeframe = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ticker);

        var url = BuildUrl($"/api/stock/{Uri.EscapeDataString(ticker)}/volatility/realized",
            ("date", date), ("timeframe", timeframe));

        var response = await _httpClient
            .GetFromJsonAsync<ApiResponse<IReadOnlyList<RealizedVolatilityData>>>(url, JsonOptions, cancellationToken)
            .ConfigureAwait(false);

        return response?.Data ?? [];
    }

    /// <summary>
    /// Returns comprehensive volatility statistics for a ticker on a given date, including
    /// current and 52-week high/low values for both implied and realized volatility,
    /// IV rank, and IV percentile.
    /// </summary>
    /// <remarks>
    /// Path: <c>GET /api/stock/{ticker}/volatility/stats</c><br/>
    /// Operation ID: <c>PublicApi.TickerController.volatility_stats</c>
    /// </remarks>
    /// <param name="ticker">The stock ticker symbol.</param>
    /// <param name="date">Optional trading date filter (YYYY-MM-DD).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The volatility statistics for the given ticker and date.</returns>
    public async Task<VolatilityStatisticsData?> GetVolatilityStatisticsAsync(
        string ticker,
        string? date = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ticker);

        var url = BuildUrl($"/api/stock/{Uri.EscapeDataString(ticker)}/volatility/stats",
            ("date", date));

        var response = await _httpClient
            .GetFromJsonAsync<ApiResponse<VolatilityStatisticsData>>(url, JsonOptions, cancellationToken)
            .ConfigureAwait(false);

        return response?.Data;
    }

    /// <summary>
    /// Returns the implied volatility term structure for a ticker — the average ATM
    /// implied volatility for every available expiry date.
    /// </summary>
    /// <remarks>
    /// Path: <c>GET /api/stock/{ticker}/volatility/term-structure</c><br/>
    /// Operation ID: <c>PublicApi.TickerController.implied_volatility_term_structure</c>
    /// </remarks>
    /// <param name="ticker">The stock ticker symbol.</param>
    /// <param name="date">Optional trading date filter (YYYY-MM-DD).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of IV term structure data points, one per expiry date.</returns>
    public async Task<IReadOnlyList<IvTermStructureData>> GetIvTermStructureAsync(
        string ticker,
        string? date = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ticker);

        var url = BuildUrl($"/api/stock/{Uri.EscapeDataString(ticker)}/volatility/term-structure",
            ("date", date));

        var response = await _httpClient
            .GetFromJsonAsync<ApiResponse<IReadOnlyList<IvTermStructureData>>>(url, JsonOptions, cancellationToken)
            .ConfigureAwait(false);

        return response?.Data ?? [];
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
