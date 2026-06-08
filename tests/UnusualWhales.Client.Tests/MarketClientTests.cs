using System.Text.Json;
using UnusualWhales.Client;
using UnusualWhales.Client.Clients;
using Xunit;

namespace UnusualWhales.Client.Tests;

/// <summary>
/// Unit tests for <see cref="MarketClient"/> that mock the HTTP layer so no
/// real network calls are made.
/// </summary>
public sealed class MarketClientTests
{
    private static (MarketClient Client, MockHttpMessageHandler Handler) CreateClient(string responseJson)
        => StocksClientTests.CreateMarketClientWithMockHandler(responseJson);

    // ── Correlations ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetCorrelationsAsync_ReturnsDeserializedData()
    {
        const string json = """
            {
                "data": [
                    {
                        "correlation": 0.27936797861890483,
                        "fst": "AAPL",
                        "snd": "SPY",
                        "min_date": "2023-07-11",
                        "max_date": "2024-07-11",
                        "rows": 253
                    },
                    {
                        "correlation": 1.0,
                        "fst": "AAPL",
                        "snd": "AAPL",
                        "min_date": "2023-07-11",
                        "max_date": "2024-07-11",
                        "rows": 253
                    }
                ]
            }
            """;

        var (client, handler) = CreateClient(json);
        var result = await client.GetCorrelationsAsync(new[] { "AAPL", "SPY" });

        Assert.Equal(2, result.Count);
        Assert.Equal("AAPL", result[0].First);
        Assert.Equal("SPY", result[0].Second);
        Assert.Equal(0.27936797861890483, result[0].Correlation, precision: 12);
        Assert.Equal(253, result[0].Rows);
        Assert.Equal("2023-07-11", result[0].MinDate);
        Assert.Equal("2024-07-11", result[0].MaxDate);

        Assert.Contains("/api/market/correlations", handler.LastRequestUri?.PathAndQuery);
        Assert.Contains("tickers=AAPL%2CSPY", handler.LastRequestUri?.Query);
    }

    [Fact]
    public async Task GetCorrelationsAsync_WithInterval_AppendsQueryParam()
    {
        var (client, handler) = CreateClient("""{"data":[]}""");
        await client.GetCorrelationsAsync(new[] { "AAPL", "SPY" }, interval: "6M");

        Assert.Contains("interval=6M", handler.LastRequestUri?.Query);
    }

    [Fact]
    public async Task GetCorrelationsAsync_WithDateRange_AppendsQueryParams()
    {
        var (client, handler) = CreateClient("""{"data":[]}""");
        await client.GetCorrelationsAsync(
            new[] { "AAPL", "SPY" },
            startDate: "2024-01-01",
            endDate: "2024-12-31");

        Assert.Contains("start_date=2024-01-01", handler.LastRequestUri?.Query);
        Assert.Contains("end_date=2024-12-31", handler.LastRequestUri?.Query);
    }

    [Fact]
    public async Task GetCorrelationsAsync_NullTickers_Throws()
    {
        var (client, _) = CreateClient("""{"data":[]}""");
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetCorrelationsAsync(null!));
    }

    [Fact]
    public async Task GetCorrelationsAsync_EmptyTickers_Throws()
    {
        var (client, _) = CreateClient("""{"data":[]}""");
        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.GetCorrelationsAsync(Array.Empty<string>()));
    }

    // ── Analytics Window ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAnalyticsWindowAsync_ReturnsDeserializedData()
    {
        const string json = """
            {
                "data": {
                    "meta": {
                        "symbols": ["AAPL", "MSFT"],
                        "range": "2month",
                        "interval": "DAILY"
                    },
                    "payload": {
                        "STDDEV": { "AAPL": 0.022, "MSFT": 0.018 },
                        "CORRELATION": { "AAPL_MSFT": 0.73 }
                    }
                }
            }
            """;

        var (client, handler) = CreateClient(json);
        var result = await client.GetAnalyticsWindowAsync(
            new[] { "AAPL", "MSFT" },
            range: "2month",
            calculations: new[] { "STDDEV", "CORRELATION" });

        Assert.NotNull(result);
        Assert.Equal(JsonValueKind.Object, result.Meta.ValueKind);
        Assert.Equal(JsonValueKind.Object, result.Payload.ValueKind);
        Assert.Equal(0.022, result.Payload.GetProperty("STDDEV").GetProperty("AAPL").GetDouble(), precision: 6);
        Assert.Equal(0.73, result.Payload.GetProperty("CORRELATION").GetProperty("AAPL_MSFT").GetDouble(), precision: 6);

        var query = handler.LastRequestUri?.Query ?? string.Empty;
        Assert.Contains("/api/analytics/window", handler.LastRequestUri?.PathAndQuery);
        Assert.Contains("symbols=AAPL%2CMSFT", query);
        Assert.Contains("range=2month", query);
        Assert.Contains("calculations=STDDEV%2CCORRELATION", query);
    }

    [Fact]
    public async Task GetAnalyticsWindowAsync_EmptySymbols_Throws()
    {
        var (client, _) = CreateClient("""{"data": null}""");
        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.GetAnalyticsWindowAsync(
                Array.Empty<string>(),
                range: "2month",
                calculations: new[] { "STDDEV" }));
    }

    [Fact]
    public async Task GetAnalyticsWindowAsync_EmptyCalculations_Throws()
    {
        var (client, _) = CreateClient("""{"data": null}""");
        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.GetAnalyticsWindowAsync(
                new[] { "AAPL" },
                range: "2month",
                calculations: Array.Empty<string>()));
    }

    [Fact]
    public async Task GetAnalyticsWindowAsync_EmptyRange_Throws()
    {
        var (client, _) = CreateClient("""{"data": null}""");
        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.GetAnalyticsWindowAsync(
                new[] { "AAPL" },
                range: "",
                calculations: new[] { "STDDEV" }));
    }

    // ── CancellationToken propagation ─────────────────────────────────────────────

    [Fact]
    public async Task GetCorrelationsAsync_PreCancelledToken_Throws()
    {
        var (client, _) = CreateClient("""{"data":[]}""");
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            client.GetCorrelationsAsync(new[] { "AAPL", "SPY" }, cancellationToken: cts.Token));
    }
}
