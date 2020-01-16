using System;

namespace nGrpc.ChatService
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string RoomName { get; set; }
        public int PlayerId { get; set; }
        public string Text { get; set; }
    }
}
