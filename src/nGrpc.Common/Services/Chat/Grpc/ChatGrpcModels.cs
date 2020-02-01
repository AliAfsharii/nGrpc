using System.Collections.Generic;

namespace nGrpc.Common
{
    public class SendChatReq
    {
        public string RoomName { get; set; }
        public string Text { get; set; }
    }
    public class SendChatRes
    {

    }

    public class GetLastChatsReq
    {
        public string RoomName { get; set; }
        public int LastChatId { get; set; }
    }
    public class GetLastChatsRes
    {
        public List<ChatMessage> ChatMessages { get; set; }
    }
}
