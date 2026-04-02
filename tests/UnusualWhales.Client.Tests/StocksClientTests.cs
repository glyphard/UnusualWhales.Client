using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using UnusualWhales.Client;
using UnusualWhales.Client.Clients;
using UnusualWhales.Client.Models;
using Xunit;

namespace UnusualWhales.Client.Tests;

/// <summary>
/// Unit tests for <see cref="StocksClient"/> that mock the HTTP layer so no
/// real network calls are made.
/// </summary>
public sealed class StocksClientTests
{
    // ── Helpers ──────────────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a <see cref="StocksClient"/> backed by a <see cref="MockHttpMessageHandler"/>
    /// that returns <paramref name="responseJson"/> for any request.
    /// </summary>
    private static (StocksClient Client, MockHttpMessageHandler Handler) CreateClientWithMockHandler(string responseJson)
    {
        var handler = new MockHttpMessageHandler(responseJson);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.unusualwhales.com") };
        var uwClient = new UnusualWhalesClient(httpClient);
        return (uwClient.Stocks, handler);
    }

    // ── Greek Exposure ────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetGreekExposureAsync_ReturnsDeserializedData()
    {
        const string json = """
            {
                "data": [
                    {
                        "date": "2024-01-15",
                        "call_charm": "1.23",
                        "call_delta": "0.55",
                        "call_gamma": "0.02",
                        "call_vanna": "0.01",
                        "put_charm": "-0.45",
                        "put_delta": "-0.45",
                        "put_gamma": "0.02",
                        "put_vanna": "-0.01"
                    }
                ]
            }
            """;

        var (client, handler) = CreateClientWithMockHandler(json);
        var result = await client.GetGreekExposureAsync("AAPL");

        Assert.Single(result);
        var item = result[0];
        Assert.Equal("2024-01-15", item.Date);
        Assert.Equal("0.55", item.CallDelta);
        Assert.Equal("0.02", item.CallGamma);
        Assert.Equal("-0.45", item.PutDelta);

        Assert.Contains("/api/stock/AAPL/greek-exposure", handler.LastRequestUri?.PathAndQuery);
    }

    [Fact]
    public async Task GetGreekExposureAsync_WithDateAndTimeframe_AppendsQueryParams()
    {
        const string json = """{"data":[]}""";
        var (client, handler) = CreateClientWithMockHandler(json);

        await client.GetGreekExposureAsync("AAPL", date: "2024-01-15", timeframe: "1Y");

        var query = handler.LastRequestUri?.Query ?? string.Empty;
        Assert.Contains("date=2024-01-15", query);
        Assert.Contains("timeframe=1Y", query);
    }

    [Fact]
    public async Task GetGreekExposureAsync_EmptyTicker_Throws()
    {
        var (client, _) = CreateClientWithMockHandler("""{"data":[]}""");
        await Assert.ThrowsAsync<ArgumentException>(() => client.GetGreekExposureAsync(""));
    }

    // ── Greek Exposure by Strike ──────────────────────────────────────────────────

    [Fact]
    public async Task GetGreekExposureByStrikeAsync_ReturnsDeserializedData()
    {
        const string json = """
            {
                "data": [
                    {
                        "strike": "150",
                        "call_gamma": "0.05",
                        "call_delta": "0.60",
                        "call_charm": "0.00",
                        "call_vanna": "0.00",
                        "put_gamma": "0.05",
                        "put_delta": "-0.40",
                        "put_charm": "0.00",
                        "put_vanna": "0.00"
                    }
                ]
            }
            """;

        var (client, handler) = CreateClientWithMockHandler(json);
        var result = await client.GetGreekExposureByStrikeAsync("AAPL");

        Assert.Single(result);
        Assert.Equal("150", result[0].Strike);
        Assert.Equal("0.05", result[0].CallGex);
        Assert.Contains("/api/stock/AAPL/greek-exposure/strike", handler.LastRequestUri?.PathAndQuery);
    }

    [Fact]
    public async Task GetGreekExposureByStrikeAsync_WithDate_AppendsQueryParam()
    {
        var (client, handler) = CreateClientWithMockHandler("""{"data":[]}""");
        await client.GetGreekExposureByStrikeAsync("AAPL", date: "2024-01-15");

        Assert.Contains("date=2024-01-15", handler.LastRequestUri?.Query);
    }

    // ── Greek Exposure by Expiry ──────────────────────────────────────────────────

    [Fact]
    public async Task GetGreekExposureByExpiryAsync_ReturnsDeserializedData()
    {
        const string json = """
            {
                "data": [
                    {
                        "expiry": "2024-03-15",
                        "call_gamma": "0.03",
                        "call_delta": "0.50",
                        "call_charm": "0.00",
                        "call_vanna": "0.00",
                        "put_gamma": "0.03",
                        "put_delta": "-0.50",
                        "put_charm": "0.00",
                        "put_vanna": "0.00"
                    }
                ]
            }
            """;

        var (client, handler) = CreateClientWithMockHandler(json);
        var result = await client.GetGreekExposureByExpiryAsync("AAPL");

        Assert.Single(result);
        Assert.Equal("2024-03-15", result[0].Expiry);
        Assert.Equal("0.03", result[0].CallGex);
        Assert.Contains("/api/stock/AAPL/greek-exposure/expiry", handler.LastRequestUri?.PathAndQuery);
    }

    // ── Greek Exposure by Strike & Expiry ─────────────────────────────────────────

    [Fact]
    public async Task GetGreekExposureByStrikeExpiryAsync_ReturnsDeserializedData()
    {
        const string json = """
            {
                "data": [
                    {
                        "expiry": "2024-03-15",
                        "strike": "155",
                        "call_gamma": "0.04",
                        "call_delta": "0.55",
                        "call_charm": "0.00",
                        "call_vanna": "0.00",
                        "put_gamma": "0.04",
                        "put_delta": "-0.45",
                        "put_charm": "0.00",
                        "put_vanna": "0.00"
                    }
                ]
            }
            """;

        var (client, handler) = CreateClientWithMockHandler(json);
        var result = await client.GetGreekExposureByStrikeExpiryAsync("AAPL", expiry: "2024-03-15");

        Assert.Single(result);
        Assert.Equal("2024-03-15", result[0].Expiry);
        Assert.Equal("155", result[0].Strike);
        Assert.Contains("/api/stock/AAPL/greek-exposure/strike-expiry", handler.LastRequestUri?.PathAndQuery);
        Assert.Contains("expiry=2024-03-15", handler.LastRequestUri?.Query);
    }

    [Fact]
    public async Task GetGreekExposureByStrikeExpiryAsync_EmptyExpiry_Throws()
    {
        var (client, _) = CreateClientWithMockHandler("""{"data":[]}""");
        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.GetGreekExposureByStrikeExpiryAsync("AAPL", expiry: ""));
    }

    // ── Technical Indicators ──────────────────────────────────────────────────────

    [Fact]
    public async Task GetTechnicalIndicatorAsync_RSI_ReturnsDeserializedData()
    {
        const string json = """
            {
                "data": [
                    {
                        "date": "2024-01-15",
                        "values": {
                            "RSI": "62.5"
                        },
                        "ticker": "AAPL",
                        "interval": "daily",
                        "indicator": "RSI",
                        "series_type": "close",
                        "time_period": 5
                    },
                    {
                        "date": "2024-01-14",
                        "values": {
                            "RSI": "58.3"
                        },
                        "ticker": "AAPL",
                        "interval": "daily",
                        "indicator": "RSI",
                        "series_type": "close",
                        "time_period": 5
                    }
                ]
            }
            """;

        var (client, handler) = CreateClientWithMockHandler(json);
        var result = await client.GetTechnicalIndicatorAsync(
            "AAPL", 
            "RSI", 
            interval: "daily", 
            timePeriod: 5, 
            seriesType: "close");

        Assert.Equal(2, result.Count);
        var item = result[0];
        Assert.Equal("2024-01-15", item.Date);
        Assert.Equal("AAPL", item.Ticker);
        Assert.Equal("RSI", item.Indicator);
        Assert.Equal("daily", item.Interval);
        Assert.Equal("close", item.SeriesType);
        Assert.Equal(5, item.TimePeriod);
        Assert.Equal("62.5", item.GetValue("RSI"));
        Assert.Equal("62.5", item.GetSingleValue());

        Assert.Contains("/api/stock/AAPL/technical-indicator/RSI", handler.LastRequestUri?.PathAndQuery);
        Assert.Contains("interval=daily", handler.LastRequestUri?.Query);
        Assert.Contains("time_period=5", handler.LastRequestUri?.Query);
        Assert.Contains("series_type=close", handler.LastRequestUri?.Query);
    }

    // ── Realized Volatility ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetRealizedVolatilityAsync_ReturnsDeserializedData()
    {
        const string json = """
            {
                "data": [
                    {
                        "date": "2024-01-15",
                        "price": "185.50",
                        "implied_volatility": "0.2850",
                        "realized_volatility": "0.2210",
                        "unshifted_rv_date": "2024-02-15"
                    }
                ]
            }
            """;

        var (client, handler) = CreateClientWithMockHandler(json);
        var result = await client.GetRealizedVolatilityAsync("AAPL");

        Assert.Single(result);
        Assert.Equal("2024-01-15", result[0].Date);
        Assert.Equal("185.50", result[0].Price);
        Assert.Equal("0.2850", result[0].ImpliedVolatility);
        Assert.Equal("0.2210", result[0].RealizedVolatility);
        Assert.Equal("2024-02-15", result[0].UnshiftedRvDate);
        Assert.Contains("/api/stock/AAPL/volatility/realized", handler.LastRequestUri?.PathAndQuery);
    }

    [Fact]
    public async Task GetRealizedVolatilityAsync_WithTimeframe_AppendsQueryParam()
    {
        var (client, handler) = CreateClientWithMockHandler("""{"data":[]}""");
        await client.GetRealizedVolatilityAsync("AAPL", timeframe: "6M");

        Assert.Contains("timeframe=6M", handler.LastRequestUri?.Query);
    }

    // ── Volatility Statistics ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetVolatilityStatisticsAsync_ReturnsDeserializedData()
    {
        const string json = """
            {
                "data": {
                    "date": "2024-01-15",
                    "ticker": "AAPL",
                    "iv": "0.2850",
                    "iv_low": "0.1800",
                    "iv_high": "0.4500",
                    "iv_rank": "45",
                    "rv": "0.2210",
                    "rv_low": "0.1500",
                    "rv_high": "0.4000"
                }
            }
            """;

        var (client, handler) = CreateClientWithMockHandler(json);
        var result = await client.GetVolatilityStatisticsAsync("AAPL");

        Assert.NotNull(result);
        Assert.Equal("2024-01-15", result.Date);
        Assert.Equal("AAPL", result.Ticker);
        Assert.Equal("0.2850", result.ImpliedVolatility);
        Assert.Equal("0.2210", result.RealizedVolatility);
        Assert.Equal("0.4500", result.IvHigh);
        Assert.Equal("0.1800", result.IvLow);
        Assert.Equal("0.4000", result.RvHigh);
        Assert.Equal("0.1500", result.RvLow);
        Assert.Equal("45", result.IvRank);
        Assert.Contains("/api/stock/AAPL/volatility/stats", handler.LastRequestUri?.PathAndQuery);
    }

    [Fact]
    public async Task GetVolatilityStatisticsAsync_ReturnsNull_WhenDataIsNull()
    {
        const string json = """{"data": null}""";
        var (client, _) = CreateClientWithMockHandler(json);
        var result = await client.GetVolatilityStatisticsAsync("AAPL");
        Assert.Null(result);
    }

    // ── IV Term Structure ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetIvTermStructureAsync_ReturnsDeserializedData()
    {
        const string json = """
            {
                "data": [
                    {
                        "date": "2024-01-15",
                        "dte": 30,
                        "expiry": "2024-02-16",
                        "implied_move": "5.80",
                        "implied_move_perc": "0.031",
                        "ticker": "AAPL",
                        "volatility": "0.2850"
                    },
                    {
                        "date": "2024-01-15",
                        "dte": 60,
                        "expiry": "2024-03-15",
                        "implied_move": "8.20",
                        "implied_move_perc": "0.044",
                        "ticker": "AAPL",
                        "volatility": "0.2950"
                    }
                ]
            }
            """;

        var (client, handler) = CreateClientWithMockHandler(json);
        var result = await client.GetIvTermStructureAsync("AAPL");

        Assert.Equal(2, result.Count);
        Assert.Equal(30, result[0].Dte);
        Assert.Equal("2024-02-16", result[0].Expiry);
        Assert.Equal("0.2850", result[0].Volatility);
        Assert.Equal(60, result[1].Dte);
        Assert.Contains("/api/stock/AAPL/volatility/term-structure", handler.LastRequestUri?.PathAndQuery);
    }

    // ── UnusualWhalesClient construction ─────────────────────────────────────────

    [Fact]
    public void Constructor_WithOptions_ValidApiKey_Succeeds()
    {
        var client = new UnusualWhalesClient(new UnusualWhalesClientOptions { ApiKey = "test-key" });
        Assert.NotNull(client.Stocks);
        client.Dispose();
    }

    [Fact]
    public void Constructor_WithOptions_EmptyApiKey_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new UnusualWhalesClient(new UnusualWhalesClientOptions { ApiKey = "" }));
    }

    [Fact]
    public void Constructor_NullOptions_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new UnusualWhalesClient((UnusualWhalesClientOptions)null!));
    }

    [Fact]
    public void Constructor_NullHttpClient_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new UnusualWhalesClient((HttpClient)null!));
    }
}

// ── Test infrastructure ───────────────────────────────────────────────────────

/// <summary>
/// A <see cref="HttpMessageHandler"/> that returns a pre-configured JSON string
/// for every request without hitting the network.
/// </summary>
internal sealed class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly string _responseJson;

    public Uri? LastRequestUri { get; private set; }

    public MockHttpMessageHandler(string responseJson)
    {
        _responseJson = responseJson;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        LastRequestUri = request.RequestUri;

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(_responseJson, Encoding.UTF8, "application/json"),
        };

        return Task.FromResult(response);
    }
}
