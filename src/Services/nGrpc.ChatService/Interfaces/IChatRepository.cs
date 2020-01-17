using nGrpc.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nGrpc.ChatService
{
    public interface IChatRepository
    {
        Task SaveChats(List<ChatMessage> chatMessages);
        Task<List<ChatMessage>> GetLastChatMessages(string roomName, int lastChatsCount);
    }
}
