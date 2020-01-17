using System.Collections.Generic;
using System.Threading.Tasks;

namespace nGrpc.ChatService
{
    public class ChatRepository : IChatRepository
    {
        public Task SaveChats(List<ChatMessage> chatMessages)
        {
            throw new System.NotImplementedException();
        }
    }
}
