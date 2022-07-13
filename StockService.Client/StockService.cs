using Flurl;
using Flurl.Http;

namespace StockService.Client
{
    public class StockService
    {
        private readonly string _baseUrl = Environment.GetEnvironmentVariable("StockServiceUrl") ?? "https://stooq.com/q/l/";

        public async Task<(string, decimal)> GetQuoteBySymbol(string symbol)
        {
            var csvData = await _baseUrl
                .SetQueryParams(new { s = symbol, f = "sd2t2ohlcv", h = "", e = "csv" })
                .GetStringAsync();

            var data = csvData.Split('\n')[1];
            var resultSymbol = data.Split(',')[0];
            var quote = data.Split(',')[6];
            if (decimal.TryParse(quote, out var resultQuote))
            {
                return (resultSymbol, resultQuote);
            }
            return (string.Empty, 0);
        }
    }
}