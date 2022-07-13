using System.Text.RegularExpressions;

namespace FinancialChat.Service.Handlers
{
    public class MessageHandler : IMessageHandler
    { 
        private readonly Regex _basicCommandRegex = new(@"^/(?<command>\w+)\s*=\s*(?<parameter>[a-zA-Z0-9_\.]*)");

        private Dictionary<string, Action> _commandList = new()
        {
            {"stock", ()=> Console.WriteLine("Hola") }
        };

        public MessageHandler()
        {
            
        }

        public bool CommandDecoder(string text, out string command, out string parameter)
        {
            command = string.Empty;
            parameter = string.Empty;
            var matches = _basicCommandRegex.Matches(text);
            if (matches.Count > 0)
            {
                command = matches.First().Groups["command"].Value;
                parameter = matches.First().Groups["parameter"].Value;
                return true;
            }
            return false;
        }
    }
}
