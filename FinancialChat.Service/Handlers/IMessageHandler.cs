namespace FinancialChat.Service.Handlers
{
    public interface IMessageHandler
    {
        bool CommandDecoder(string text, out string command, out string parameter);
    }
}
