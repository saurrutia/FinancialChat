using FinancialChat.Service.Handlers;

namespace FinancialChat.UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void ItShouldSplitCommandAndParameter()
        {
            var handler = new MessageHandler();
            var response = handler.CommandDecoder("/stock=aapl.us", out var command, out var param);
            Assert.Equal("stock aapl.us", $"{command} {param}");
        }

        [Fact]
        public async Task Test2()
        {
            var stockService = new StockService.Client.StockService();
            var result = await stockService.GetQuoteBySymbol("aapl.us");
            Assert.Equal("AAPL.US", result.Item1);
            Assert.True(result.Item2 > 0);
        }
    }
}