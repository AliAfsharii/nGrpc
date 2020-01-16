using System;

namespace nGrpc.ChatService
{
    public class PlayerHasNotJoinedToChatRoomException : Exception
    {
        public PlayerHasNotJoinedToChatRoomException()
        {
        }

        public PlayerHasNotJoinedToChatRoomException(string message) : base(message)
        {
        }
    }
}
