using Microsoft.Extensions.Logging;
using nGrpc.ServerCommon;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace nGrpc.ChatService
{
    public class ChatRoom
    {
        private string _roomName;
        public string RoomName
        {
            get
            {
                return _roomName;
            }
            set
            {
                if (_roomName == null)
                    _roomName = value;
                else
                    throw new Exception("RoomName has set earlier");
            }
        }

        private readonly ILogger<ChatRoom> _logger;
        private readonly DateTime _createDate;
        private readonly ITime _time;
        private readonly ChatConfigs _chatConfigs;
        private readonly IChatRepository _chatRepository;
        private readonly IPubSubHub _pubSubHub;
        private readonly AsyncLock _asyncLock = new AsyncLock();
        private readonly List<int> _playersIds = new List<int>();
        private readonly List<ChatMessage> _chatMessages = new List<ChatMessage>();

        private int _lastChatId = 0;
        private int _lastSavedChatId;

        public ChatRoom(ILogger<ChatRoom> logger,
            ITime time,
            ChatConfigs chatConfigs,
            IChatRepository chatRepository,
            IPubSubHub pubSubHub)
        {
            _logger = logger;
            _time = time;
            _createDate = _time.UTCTime;
            _chatConfigs = chatConfigs;
            _chatRepository = chatRepository;
            _pubSubHub = pubSubHub;

            Scheduler().NoAwait();
        }


        private async Task Scheduler()
        {
            try
            {
                while (true)
                {
                    await Task.Delay(_chatConfigs.ChatSaveIntervalInMilisec);

                    await SaveNewChatMessagesToDB();
                    RemoveRedundantCachedChatMessages();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private void RemoveRedundantCachedChatMessages()
        {
            using (_asyncLock.Lock())
            {
                if (_chatMessages.Count > _chatConfigs.ChatGetLatestChatsCount)
                {
                    int redundantChatsCount = _chatMessages.Count - _chatConfigs.ChatGetLatestChatsCount;
                    _chatMessages.RemoveRange(0, redundantChatsCount);
                }
            }
        }

        private async Task SaveNewChatMessagesToDB()
        {
            List<ChatMessage> shouldSaveChats;
            using (await _asyncLock.LockAsync())
                shouldSaveChats = _chatMessages.Where(n => n.Id > _lastSavedChatId).ToList();

            if (shouldSaveChats.Count > 0)
            {
                await _chatRepository.SaveChats(shouldSaveChats);
                _lastSavedChatId = shouldSaveChats.Last().Id;
            }
        }


        public void AddPlayer(int playerId)
        {
            using (_asyncLock.Lock())
                if (_playersIds.Contains(playerId) == false)
                    _playersIds.Add(playerId);
        }

        //public List<int> GetPlayersIds()
        //{
        //    using (_asyncLock.Lock())
        //        return _playersIds.CloneByMessagePack();
        //}

        public bool IsPlayerInRoom(int playerId)
        {
            using (_asyncLock.Lock())
                return _playersIds.Contains(playerId);
        }

        public void AddChatMessage(int playerId, string text)
        {
            ChatMessage chatMessage = new ChatMessage
            {
                Id = Interlocked.Increment(ref _lastChatId),
                Date = _time.UTCTime,
                RoomName = _roomName,
                PlayerId = playerId,
                Text = text
            };

            using (_asyncLock.Lock())
                _chatMessages.Add(chatMessage);

            ChatSentMessage chatSentMessage = new ChatSentMessage
            {
                ChatRoomPlayersIds = _playersIds.CloneByMessagePack(),
                ChatMessage = chatMessage
            };
            _pubSubHub.Publish(chatSentMessage);
        }

        public List<ChatMessage> GetLatestChatMessages(int playerId, int lastChatId)
        {
            return _chatMessages.Where(n => n.Id > lastChatId).Take(_chatConfigs.ChatGetLatestChatsCount).ToList();
        }
    }
}