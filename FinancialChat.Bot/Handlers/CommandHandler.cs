using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialChat.Bot.Handlers
{
    public class CommandHandler
    {
        private readonly Dictionary<string, Func<string, Task<CommandResult>>> _commandAction;
        private readonly StockService.Client.StockService _stockService;

        public CommandHandler()
        {
            _stockService = new();
            _commandAction = new()
            {
                { "stock", async (param) => {
                    var result = await _stockService.GetQuoteBySymbol(param.Split(' ')[1]);
                    if(result.Item1 == string.Empty && result.Item2 == 0)
                    {
                        return new CommandResult { Result = $"No quote found for {param.Split(' ')[1]}." };
                    }
                    return new CommandResult { Result = $"{result.Item1} quote is ${result.Item2} per share." };
                } }
            };
        }

        public async Task<CommandResult> Handle(string command)
        {
            if (_commandAction.TryGetValue(command.Split(' ')[0], out var result))
            {
                return await result.Invoke(command);
            }
            return new CommandResult();
        }
    }

    public class CommandResult
    {
        public string Result { get; set; }
    }
}
