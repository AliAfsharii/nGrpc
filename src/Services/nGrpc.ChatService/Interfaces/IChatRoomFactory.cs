using System;
using System.Collections.Generic;
using System.Text;

namespace nGrpc.ChatService.Interfaces
{
    public interface IChatRoomFactory
    {
        ChatRoom CreateNewChatRoom();
    }
}
