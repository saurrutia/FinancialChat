using Grpc.Core;
using static FinancialChat.Service.RequestMessage.Types;

namespace FinancialChat.Service.Handlers
{
    public interface IChatServiceHandler
    {
        public void Join(string name, ClientType clientType, IServerStreamWriter<ResponseMessage> responseStream);
        public void Remove(string name, ClientType clientType);
        public Task SendMessage(RequestMessage message);
    }
}
