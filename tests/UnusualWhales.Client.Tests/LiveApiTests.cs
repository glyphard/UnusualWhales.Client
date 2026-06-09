using System.Globalization;
using UnusualWhales.Client;
using Xunit;

namespace UnusualWhales.Client.Tests;

/// <summary>
/// Live tests that hit the real Unusual Whales API. They validate every
/// public method that has been added or reshaped since NuGet package
/// v1.0.0 actually deserializes against production responses (the mocked
/// unit tests can drift from real payload shapes — e.g. <c>correlation</c>
/// is returned as a JSON string, not a number).
///
/// The key is read from the <c>UW_API_KEY</c> environment variable so it
/// never lands in source control. When the variable is unset, each test is
/// skipped rather than failed.
/// </summary>
public sealed class LiveApiTests
{
    private const string ApiKeyEnvVar = "UW_API_KEY";
    private const string SampleTicker = "AAPL";

    private static UnusualWhalesClient CreateClient()
    {
        var apiKey = Environment.GetEnvironmentVariable(ApiKeyEnvVar);
        Skip.If(string.IsNullOrWhiteSpace(apiKey),
            $"{ApiKeyEnvVar} environment variable not set — skipping live API test.");

        return new UnusualWhalesClient(new UnusualWhalesClientOptions { ApiKey = apiKey! });
    }

    private static double ParseDouble(string s)
        => double.Parse(s, NumberStyles.Float, CultureInfo.InvariantCulture);

    // ── MarketClient (new since v1.0.0) ───────────────────────────────────────────

    [SkippableFact, Trait("Category", "Unit")]
    public async Task GetCorrelationsAsync_LiveApi_ReturnsPairsForBasket()
    {
        using var client = CreateClient();

        var result = await client.Market.GetCorrelationsAsync(
            new[] { "AAPL", "SPY" },
            interval: "6M");

        Assert.NotEmpty(result);

        var aaplSpy = result.FirstOrDefault(c => c.First == "AAPL" && c.Second == "SPY");
        Assert.NotNull(aaplSpy);
        Assert.InRange(aaplSpy!.Correlation, -1d, 1d);
        Assert.True(aaplSpy.Rows > 0);
        Assert.False(string.IsNullOrWhiteSpace(aaplSpy.MinDate));
        Assert.False(string.IsNullOrWhiteSpace(aaplSpy.MaxDate));
    }

    [SkippableFact, Trait("Category", "Unit")]
    public async Task GetCorrelationsAsync_LiveApi_ThreeTickerBasket_CoversAllPairs()
    {
        using var client = CreateClient();

        var result = await client.Market.GetCorrelationsAsync(
            new[] { "AAPL", "MSFT", "SPY" },
            interval: "1M");

        Assert.NotEmpty(result);
        Assert.Contains(result, c =>
            (c.First == "AAPL" && c.Second == "MSFT") ||
            (c.First == "MSFT" && c.Second == "AAPL"));

        foreach (var pair in result)
            Assert.InRange(pair.Correlation, -1d, 1d);
    }

    [SkippableFact, Trait("Category", "Unit")]
#pragma warning disable CS0618 // intentionally exercising the obsolete Advanced+ method
    public async Task GetAnalyticsWindowAsync_LiveApi_ReturnsPayloadOrTierError()
    {
        using var client = CreateClient();

        // This endpoint is Advanced+ tier — a non-Advanced key surfaces as
        // an HttpRequestException from GetFromJsonAsync. We accept either
        // outcome as a "live deserialization works" signal: success means
        // payload deserialized; the tier error means we reached the server
        // and the HTTP layer is wired up correctly.
        try
        {
            var result = await client.Market.GetAnalyticsWindowAsync(
                new[] { "AAPL", "MSFT" },
                range: "1month",
                calculations: new[] { "STDDEV", "CORRELATION" });

            Assert.NotNull(result);
            Assert.Equal(System.Text.Json.JsonValueKind.Object, result!.Payload.ValueKind);
        }
        catch (HttpRequestException ex) when (
            ex.Message.Contains("403") ||
            ex.Message.Contains("402") ||
            ex.Message.Contains("advanced", StringComparison.OrdinalIgnoreCase))
        {
            // Tier-gated for this key. Treat as a soft skip — the request
            // was well-formed enough to reach the tier check.
            Skip.If(true, "Analytics Window requires Advanced+ tier for this key.");
        }
    }
#pragma warning restore CS0618

    // ── StocksClient: GetTickerInfoAsync (new since v1.0.0) ───────────────────────

    [SkippableFact, Trait("Category", "Unit")]
    public async Task GetTickerInfoAsync_LiveApi_AAPL_IncludesBeta()
    {
        using var client = CreateClient();

        var info = await client.Stocks.GetTickerInfoAsync(SampleTicker);

        Assert.NotNull(info);
        Assert.Equal(SampleTicker, info!.Symbol);
        Assert.False(string.IsNullOrWhiteSpace(info.Beta));
        Assert.True(ParseDouble(info.Beta!) > 0, "Expected a positive beta.");
        Assert.Equal("Technology", info.Sector);
        Assert.True(info.HasOptions);
    }

    [SkippableFact, Trait("Category", "Unit")]
    public async Task GetTickerInfoAsync_LiveApi_SPY_IsAvailable()
    {
        using var client = CreateClient();

        var info = await client.Stocks.GetTickerInfoAsync("SPY");

        Assert.NotNull(info);
        Assert.Equal("SPY", info!.Symbol);
        Assert.False(string.IsNullOrWhiteSpace(info.Beta));
        _ = ParseDouble(info.Beta!);
    }

    // ── StocksClient: GetTechnicalIndicatorAsync (reshaped since v1.0.0) ─────────
    // The 1.0.0 release exposed GetTechnicalIndicatorsAsync hitting
    // /technical-indicators (no function path segment). Since v1.0.0 it was
    // renamed and reshaped to take a function path segment and four query
    // parameters, hitting /technical-indicator/{function}.

    [SkippableFact, Trait("Category", "Unit")]
    public async Task GetTechnicalIndicatorAsync_LiveApi_RSI_ReturnsSeries()
    {
        using var client = CreateClient();

        var result = await client.Stocks.GetTechnicalIndicatorAsync(
            SampleTicker,
            function: "RSI",
            interval: "daily",
            timePeriod: 14,
            seriesType: "close");

        Assert.NotEmpty(result);

        var first = result[0];
        Assert.Equal(SampleTicker, first.Ticker);
        Assert.Equal("RSI", first.Indicator);
        Assert.Equal("daily", first.Interval);
        Assert.Equal(14, first.TimePeriod);
        Assert.False(string.IsNullOrWhiteSpace(first.Date));

        var rsi = first.GetValue("RSI");
        Assert.False(string.IsNullOrWhiteSpace(rsi));
        Assert.InRange(ParseDouble(rsi!), 0d, 100d);
    }

    [SkippableFact, Trait("Category", "Unit")]
    public async Task GetTechnicalIndicatorAsync_LiveApi_MACD_ReturnsMultiValueDict()
    {
        using var client = CreateClient();

        var result = await client.Stocks.GetTechnicalIndicatorAsync(
            SampleTicker,
            function: "MACD",
            interval: "daily",
            seriesType: "close");

        Assert.NotEmpty(result);

        // MACD returns three keys in the values dict: MACD, MACD_Hist, MACD_Signal.
        var first = result[0];
        Assert.Equal("MACD", first.Indicator);
        Assert.False(string.IsNullOrWhiteSpace(first.GetValue("MACD")));
        Assert.False(string.IsNullOrWhiteSpace(first.GetValue("MACD_Signal")));
        Assert.False(string.IsNullOrWhiteSpace(first.GetValue("MACD_Hist")));
    }

    // ── StocksClient: greek-exposure family (model fields renamed/added since v1.0.0)

    [SkippableFact, Trait("Category", "Unit")]
    public async Task GetGreekExposureAsync_LiveApi_DailySeries()
    {
        using var client = CreateClient();

        var result = await client.Stocks.GetGreekExposureAsync(SampleTicker);

        Assert.NotEmpty(result);
        var item = result[0];
        Assert.False(string.IsNullOrWhiteSpace(item.Date));
        Assert.False(string.IsNullOrWhiteSpace(item.CallDelta));
        Assert.False(string.IsNullOrWhiteSpace(item.PutDelta));
        Assert.False(string.IsNullOrWhiteSpace(item.CallGamma));
        Assert.False(string.IsNullOrWhiteSpace(item.PutGamma));

        // Sanity-check that the string fields actually parse as numbers.
        _ = ParseDouble(item.CallDelta);
        _ = ParseDouble(item.CallGamma);
    }

    [SkippableFact, Trait("Category", "Unit")]
    public async Task GetGreekExposureByStrikeAsync_LiveApi_ByStrike()
    {
        using var client = CreateClient();

        var result = await client.Stocks.GetGreekExposureByStrikeAsync(SampleTicker);

        Assert.NotEmpty(result);
        var item = result[0];
        Assert.False(string.IsNullOrWhiteSpace(item.Strike));
        Assert.False(string.IsNullOrWhiteSpace(item.CallGex));
        Assert.False(string.IsNullOrWhiteSpace(item.PutGex));
        _ = ParseDouble(item.Strike);
    }

    [SkippableFact, Trait("Category", "Unit")]
    public async Task GetGreekExposureByExpiryAsync_LiveApi_ByExpiryWithDte()
    {
        using var client = CreateClient();

        var result = await client.Stocks.GetGreekExposureByExpiryAsync(SampleTicker);

        Assert.NotEmpty(result);
        var item = result[0];
        Assert.False(string.IsNullOrWhiteSpace(item.Expiry));
        Assert.True(item.Dte >= 0, "DTE must be non-negative.");
        Assert.False(string.IsNullOrWhiteSpace(item.CallGex));
        Assert.False(string.IsNullOrWhiteSpace(item.PutGex));
    }

    [SkippableFact, Trait("Category", "Unit")]
    public async Task GetGreekExposureByStrikeExpiryAsync_LiveApi_ByStrikeAndExpiry()
    {
        using var client = CreateClient();

        // First find a valid expiry from the by-expiry endpoint so the test
        // doesn't depend on a hard-coded date.
        var expiries = await client.Stocks.GetGreekExposureByExpiryAsync(SampleTicker);
        Skip.If(expiries.Count == 0, "No expiries returned by /greek-exposure/expiry to chain off of.");
        var firstExpiry = expiries[0].Expiry;

        var result = await client.Stocks.GetGreekExposureByStrikeExpiryAsync(
            SampleTicker, expiry: firstExpiry);

        Assert.NotEmpty(result);
        var item = result[0];
        Assert.Equal(firstExpiry, item.Expiry);
        Assert.False(string.IsNullOrWhiteSpace(item.Strike));
        Assert.False(string.IsNullOrWhiteSpace(item.CallGex));
        Assert.False(string.IsNullOrWhiteSpace(item.PutGex));
    }

    // ── StocksClient: volatility family (model fields changed since v1.0.0) ──────

    [SkippableFact, Trait("Category", "Unit")]
    public async Task GetRealizedVolatilityAsync_LiveApi_ReturnsTimeSeries()
    {
        using var client = CreateClient();

        var result = await client.Stocks.GetRealizedVolatilityAsync(SampleTicker);

        Assert.NotEmpty(result);
        var item = result[0];
        Assert.False(string.IsNullOrWhiteSpace(item.Date));
        Assert.False(string.IsNullOrWhiteSpace(item.Price));
        Assert.False(string.IsNullOrWhiteSpace(item.ImpliedVolatility));
        Assert.False(string.IsNullOrWhiteSpace(item.RealizedVolatility));

        Assert.True(ParseDouble(item.Price) > 0);
        Assert.True(ParseDouble(item.ImpliedVolatility) > 0);
    }

    [SkippableFact, Trait("Category", "Unit")]
    public async Task GetVolatilityStatisticsAsync_LiveApi_ReturnsCurrentSnapshot()
    {
        using var client = CreateClient();

        var stats = await client.Stocks.GetVolatilityStatisticsAsync(SampleTicker);

        Assert.NotNull(stats);
        Assert.Equal(SampleTicker, stats!.Ticker);
        Assert.False(string.IsNullOrWhiteSpace(stats.Date));
        Assert.False(string.IsNullOrWhiteSpace(stats.ImpliedVolatility));
        Assert.False(string.IsNullOrWhiteSpace(stats.RealizedVolatility));
        Assert.False(string.IsNullOrWhiteSpace(stats.IvHigh));
        Assert.False(string.IsNullOrWhiteSpace(stats.IvLow));
        Assert.False(string.IsNullOrWhiteSpace(stats.IvRank));

        var ivHigh = ParseDouble(stats.IvHigh!);
        var ivLow = ParseDouble(stats.IvLow!);
        Assert.True(ivHigh >= ivLow, "iv_high should be >= iv_low.");
    }

    [SkippableFact, Trait("Category", "Unit")]
    public async Task GetIvTermStructureAsync_LiveApi_ReturnsAcrossExpiries()
    {
        using var client = CreateClient();

        var result = await client.Stocks.GetIvTermStructureAsync(SampleTicker);

        Assert.NotEmpty(result);
        var item = result[0];
        Assert.False(string.IsNullOrWhiteSpace(item.Expiry));
        Assert.False(string.IsNullOrWhiteSpace(item.Volatility));
        Assert.True(item.Dte >= 0);
        _ = ParseDouble(item.Volatility!);
    }
}
