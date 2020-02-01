using nGrpc.Common;
using System.Collections.Generic;

namespace nGrpc.ServerCommon
{
    public interface IChatHub
    {
        List<ChatMessage> GetLastChats(int playerId, string roomName, int lastChatId);
        bool IsPlayerJoined(int playerId, string roomName);
        void JoinRoom(int playerId, string roomName);
        void LeaveRoom(int playerId, string roomName);
        void SendChat(int playerId, string roomName, string text);
    }
}