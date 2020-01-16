using nGrpc.ServerCommon;
using Nito.AsyncEx;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace nGrpc.ChatService
{
    public class ChatHub
    {
        private readonly IPubSubHub _pubSubHub;
        private readonly ITime _time;
        private readonly ChatConfigs _chatConfigs;

        public ChatHub(
            IPubSubHub pubSubHub,
            ITime time,
            ChatConfigs chatConfigs)
        {
            _pubSubHub = pubSubHub;
            _time = time;
            _chatConfigs = chatConfigs;
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
                        chatRoom = new ChatRoom(roomName, _time, _chatConfigs);
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

        private bool CheckPlayerIsInRoom(int playerId, string roomName)
        {
            ChatRoom chatRoom = GetChatRoom(playerId, roomName);
            bool b = chatRoom.IsPlayerInRoom(playerId);
            return b;
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
            //if (CheckPlayerIsInRoom(playerId, roomName) == false)
            //    throw new PlayerHasNotJoinedToChatRoomException($"PlayerId:{playerId}, RoomName:{roomName}");

            ChatRoom chatRoom = GetChatRoom(playerId, roomName);
            ChatMessage chatMessage = chatRoom.AddChatMessage(playerId, text);

            ChatSentMessage chatSentMessage = new ChatSentMessage
            {
                ChatRoomPlayersIds = chatRoom.GetPlayersId(),
                ChatMessage = chatMessage
            };
            _pubSubHub.Publish(chatSentMessage);
        }

        public async Task<List<ChatMessage>> GetLastChats(int playerId, string roomName, int lastChatId)
        {
            ChatRoom chatRoom = GetChatRoom(playerId, roomName);
            return chatRoom.GetLatestChatMessages(playerId, lastChatId);
        }




        class ChatRoom
        {
            private readonly string _roomName;
            private readonly DateTime _createDate;
            private readonly ITime _time;
            private readonly ChatConfigs _chatConfigs;
            private readonly AsyncLock _asyncLock = new AsyncLock();
            private int _lastChatId = 0;
            private readonly List<int> _playersIds = new List<int>();
            private readonly ConcurrentDictionary<int, ChatMessage> _chatIdTochatMessages = new ConcurrentDictionary<int, ChatMessage>();

            public ChatRoom(string name,
                ITime time,
                ChatConfigs chatConfigs)
            {
                _roomName = name;
                _time = time;
                _chatConfigs = chatConfigs;
            }


            public void AddPlayer(int playerId)
            {
                using (_asyncLock.Lock())
                    if (_playersIds.Contains(playerId) == false)
                        _playersIds.Add(playerId);
            }

            public List<int> GetPlayersId()
            {
                using (_asyncLock.Lock())
                    return _playersIds.CloneByMessagePack();
            }

            public bool IsPlayerInRoom(int playerId)
            {
                using (_asyncLock.Lock())
                    return _playersIds.Contains(playerId);
            }

            public ChatMessage AddChatMessage(int playerId, string text)
            {
                ChatMessage chatMessage = new ChatMessage
                {
                    Id = Interlocked.Increment(ref _lastChatId),
                    Date = _time.UTCTime,
                    RoomName = _roomName,
                    PlayerId = playerId,
                    Text = text
                };

                _chatIdTochatMessages.TryAdd(chatMessage.Id, chatMessage);

                return chatMessage;
            }

            public List<ChatMessage> GetLatestChatMessages(int playerId, int lastChatId)
            {
                return _chatIdTochatMessages.Values.Where(n => n.Id > lastChatId).Take(_chatConfigs.ChatGetLatestChatsCount).ToList();
            }
        }
    }
}
