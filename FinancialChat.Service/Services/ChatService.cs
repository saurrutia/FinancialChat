using FinancialChat.Service;
using FinancialChat.Service.Handlers;
using Grpc.Core;

namespace FinancialChat.Service.Services
{
    public class ChatService : ChatRoom.ChatRoomBase
    {
        private readonly IChatServiceHandler _chatServiceHandler;
        public ChatService(ILogger<ChatService> logger, IChatServiceHandler chatServiceHandler)
        {
            _chatServiceHandler = chatServiceHandler;
        }

        public override async Task Join(IAsyncStreamReader<RequestMessage> requestStream, IServerStreamWriter<ResponseMessage> responseStream, ServerCallContext context)
        {
            if (!await requestStream.MoveNext()) return;
            var name = requestStream.Current.Name;
            var clientType = requestStream.Current.ClientType;
            do
            {
                _chatServiceHandler.Join(requestStream.Current.Name, requestStream.Current.ClientType, responseStream);
                await _chatServiceHandler.SendMessage(requestStream.Current);
            } while (await requestStream.MoveNext());

            _chatServiceHandler.Remove(name, clientType);
        }
    }
}