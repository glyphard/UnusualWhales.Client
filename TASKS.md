# UnusualWhales.Client — Open Tasks

## 1. Add Beta vs SPX to `StocksClient`

Expose the Unusual Whales beta endpoint for a ticker measured against SPX.

- Endpoint: `GET /api/stock/{ticker}/beta` (verify exact path against UW API docs — may also be exposed under `/volatility/stats` or as a dedicated `/beta-vs-spx` route).
- New model: `BetaData` (or extend `VolatilityStatisticsData` if beta is already returned there — check the JSON shape before deciding).
- New method on `StocksClient`:
  ```csharp
  Task<IReadOnlyList<BetaData>> GetBetaAsync(
      string ticker,
      string? date = null,
      string? timeframe = null,
      CancellationToken cancellationToken = default)
  ```
- Follow the existing `GetRealizedVolatilityAsync` pattern: `ArgumentException.ThrowIfNullOrWhiteSpace(ticker)`, `BuildUrl(...)`, `GetFromJsonAsync<ApiResponse<...>>`.
- Add XML docs with the Path and Operation ID.
- Add a unit test alongside the existing `tests/` project.

## 2. Add `MarketClient` for basket/cross-sectional endpoints

No `MarketClient` exists today. Add one to surface market-wide and multi-ticker endpoints.

- New file: `src/UnusualWhales.Client/Clients/MarketClient.cs` (mirror `StocksClient` structure — `internal` ctor taking `HttpClient`, shared `JsonOptions`, `BuildUrl` helper).
- Wire it into `UnusualWhalesClient` as a new property: `public MarketClient Market { get; }`. Initialize in both constructors next to `Stocks`.
- Endpoints to cover (verify each against UW API docs):
  - **Correlation matrix** for a basket of tickers — likely `GET /api/market/correlations?tickers=AAPL,MSFT,...` or similar.
  - **Volatilities for a basket** — implied/realized volatility for multiple tickers in one call.
  - Any other useful market-wide endpoints encountered while documenting the above (e.g. sector ETF stats, market tide, SPIKE).
- New models in `Models/`:
  - `CorrelationMatrixData` (consider shape: list of `{ ticker, ticker, value }` triples vs. nested dict — match API response).
  - `BasketVolatilityData`.
- Method signatures should accept `IEnumerable<string> tickers` and join them server-side appropriately (comma-separated query param is the UW convention).
- Tests in `tests/` covering the happy path and empty-ticker-list guard.

## 3. Audit and extend `CancellationToken` support

Cancellation tokens are already present on every public method in `StocksClient` and are forwarded into `GetFromJsonAsync`. Treat this task as an audit + propagation step rather than a from-scratch addition.

- Audit: confirm every public async method across all client classes (current + new `MarketClient`) accepts a `CancellationToken cancellationToken = default` parameter and forwards it to the inner `HttpClient` call.
- Ensure new code added for tasks (1) and (2) follows the same pattern — do not add methods without a `CancellationToken` parameter.
- Consider adding `.ConfigureAwait(false)` consistently (already done in `StocksClient`).
- No changes expected to `UnusualWhalesClient.cs` itself — it does not perform any awaits.

## Cross-cutting

- Update `README.md` with usage snippets for the new endpoints once implemented.
- Bump package version in `.csproj` before the next NuGet publish (see commit `b31e892` for the prior prep pattern).
