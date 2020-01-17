using nGrpc.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nGrpc.ChatService
{
    public class ChatRepository : IChatRepository
    {
        public async Task<List<ChatMessage>> GetLastChatMessages(string roomName, int lastChatsCount)
        {
            return new List<ChatMessage>();
        }

        public async Task SaveChats(List<ChatMessage> chatMessages)
        {
            
        }
    }
}
