using Microsoft.Extensions.Logging;
using nGrpc.ServerCommon;

namespace nGrpc.ChatService
{
    public class ChatRoomFactory : IChatRoomFactory
    {
        private readonly ILogger<ChatRoom> _logger;
        private readonly ITime _time;
        private readonly ChatConfigs _chatConfigs;
        private readonly IChatRepository _chatRepository;
        private readonly IPubSubHub _pubsub;

        public ChatRoomFactory(
            ILogger<ChatRoom> logger,
            ITime time,
            ChatConfigs chatConfigs,
            IChatRepository chatRepository,
            IPubSubHub pubsub)
        {
            _logger = logger;
            _time = time;
            _chatConfigs = chatConfigs;
            _chatRepository = chatRepository;
            _pubsub = pubsub;
        }

        public ChatRoom CreateNewChatRoom(string roomName)
        {
            ChatRoom chatRoom = new ChatRoom(_logger, roomName, _time, _chatConfigs, _chatRepository, _pubsub);
            return chatRoom;
        }
    }
}
