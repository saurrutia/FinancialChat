using FinancialChat.Common;
using FinancialChat.Domain.Entities;
using FinancialChat.Domain.Repositories;
using FinancialChat.Service;
using FinancialChat.Service.Handlers;
using Grpc.Core;

namespace FinancialChat.Service.Services
{
    public class ChatService : ChatRoom.ChatRoomBase
    {
        private readonly IChatServiceHandler _chatServiceHandler;
        private readonly IRepository<User> _userRepository;
        public ChatService(ILogger<ChatService> logger, IChatServiceHandler chatServiceHandler, IRepository<User> userRepository)
        {
            _chatServiceHandler = chatServiceHandler;
            _userRepository = userRepository;
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

        public override async Task<ResponseLogin> Login(RequestLogin request, ServerCallContext context)
        {
            var user = await _userRepository.FindByConditionReadOnlyFirstOrDefault(u => u.Username == request.Username && u.Password == request.Password.ToSha256());
            if (user == null)
            {
                return new ResponseLogin { Ok = false };
            }
            return new ResponseLogin { Ok = true };
        }
    }
}