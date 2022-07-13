using Grpc.Core;
using System.Collections.Concurrent;
using static FinancialChat.Service.RequestMessage.Types;

namespace FinancialChat.Service.Handlers
{
    public class ChatServiceHandler : IChatServiceHandler
    {
        private readonly IMessageHandler _messageHandler;
        private readonly List<ConcurrentDictionary<string, IServerStreamWriter<ResponseMessage>>> _clients = new()
        {
            new ConcurrentDictionary<string, IServerStreamWriter<ResponseMessage>>(), //users
            new ConcurrentDictionary<string, IServerStreamWriter<ResponseMessage>>()  //robots
        };

        public ChatServiceHandler(IMessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
        }      

        public void Join(string name, ClientType clientType, IServerStreamWriter<ResponseMessage> responseStream)
            => _clients[(int)clientType].TryAdd(name, responseStream);

        public void Remove(string name, ClientType clientType)
            => _clients[(int)clientType].TryRemove(name, out var _);

        public async Task SendMessage(RequestMessage message)
        {
            if (_messageHandler.CommandDecoder(message.Text, out var command, out var parameter))
            {
                await SendMessageToRobots($"{command} {parameter}");
            }
            await SendMessageToUsers(message);
        }

        private async Task SendMessageToRobots(string message)
        {
            foreach (var robot in _clients[(int)ClientType.Robot])
            {
                await WriteMessageToStream(robot.Value, message);
            }
        }

        private async Task SendMessageToUsers(RequestMessage message)
        {
            foreach (var user in _clients[(int)ClientType.User].Where(x => x.Key != message.Name))
            {
                await WriteMessageToStream(user.Value, $"* {message.Name}: {message.Text}");
            }
        }

        private static async Task WriteMessageToStream(IServerStreamWriter<ResponseMessage> userStream, string message)
        {
            await userStream.WriteAsync(new ResponseMessage { Text = message });
        }
}
}
