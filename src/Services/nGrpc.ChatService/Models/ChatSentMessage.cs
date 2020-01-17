using nGrpc.Common;
using System.Collections.Generic;

namespace nGrpc.ChatService
{
    public class ChatSentMessage
    {
        public List<int> ChatRoomPlayersIds { get; set; }
        public ChatMessage ChatMessage { get; set; }
    }
}
