using Nito.AsyncEx;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nGrpc.ChatService
{
    public partial class ChatHub
    {
        private readonly Func<ChatRoom> _chatRoomFactory;

        public ChatHub(Func<ChatRoom> chatRoomFactory)
        {
            _chatRoomFactory = chatRoomFactory;
        }



        // private

        AsyncLock _asyncLock = new AsyncLock();
        ConcurrentDictionary<string, ChatRoom> _roomNameToRoomClass = new ConcurrentDictionary<string, ChatRoom>();

        private ChatRoom GetOrAddChatRoom(string roomName)
        {
            if (_roomNameToRoomClass.TryGetValue(roomName, out ChatRoom chatRoom) == false)
            {
                using (_asyncLock.Lock())
                {
                    if (_roomNameToRoomClass.TryGetValue(roomName, out chatRoom) == false)
                    {
                        chatRoom = _chatRoomFactory();
                        chatRoom.RoomName = roomName;
                        _roomNameToRoomClass.TryAdd(roomName, chatRoom);
                    }
                }
            }
            return chatRoom;
        }

        private ChatRoom GetChatRoom(int playerId, string roomName)
        {
            if (_roomNameToRoomClass.TryGetValue(roomName, out ChatRoom chatRoom) == true)
            {
                if (chatRoom.IsPlayerInRoom(playerId) == false)
                    throw new PlayerHasNotJoinedToChatRoomException($"PlayerId:{playerId}, RoomName:{roomName}");
                return chatRoom;
            }
            throw new PlayerHasNotJoinedToChatRoomException($"There is Not Such Room. PlayerId:{playerId}, RoomName:{roomName}");
        }


        // public

        public void Join(int playerId, string roomName)
        {
            ChatRoom chatRoom = GetOrAddChatRoom(roomName);
            chatRoom.AddPlayer(playerId);
        }

        public bool IsPlayerJoined(int playerId, string roomName)
        {
            bool res = false;
            if (_roomNameToRoomClass.TryGetValue(roomName, out ChatRoom chatRoom) == true)
                if (chatRoom.IsPlayerInRoom(playerId) == true)
                    res = true;
            return res;
        }

        public async Task SendChat(int playerId, string roomName, string text)
        {
            ChatRoom chatRoom = GetChatRoom(playerId, roomName);
            chatRoom.AddChatMessage(playerId, text);
        }

        public async Task<List<ChatMessage>> GetLastChats(int playerId, string roomName, int lastChatId)
        {
            ChatRoom chatRoom = GetChatRoom(playerId, roomName);
            return chatRoom.GetLatestChatMessages(playerId, lastChatId);
        }
    }
}
