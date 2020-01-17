using System.Collections.Generic;
using System.Threading.Tasks;

namespace nGrpc.ChatService
{
    public class ChatRepository : IChatRepository
    {
        public Task<List<ChatMessage>> GetLastChatMessages(string roomName, int lastChatsCount)
        {
            throw new System.NotImplementedException();
        }

        public Task SaveChats(List<ChatMessage> chatMessages)
        {
            throw new System.NotImplementedException();
        }
    }
}
