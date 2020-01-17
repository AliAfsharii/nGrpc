using System.Collections.Generic;
using System.Threading.Tasks;

namespace nGrpc.ChatService
{
    public interface IChatRepository
    {
        Task SaveChats(List<ChatMessage> chatMessages);
    }
}
