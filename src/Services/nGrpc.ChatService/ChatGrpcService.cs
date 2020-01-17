using Grpc.Core;
using Microsoft.Extensions.Logging;
using nGrpc.Common;
using nGrpc.Common.Descriptors;
using nGrpc.ServerCommon;
using System;
using System.Threading.Tasks;

namespace nGrpc.ChatService
{
    public class ChatGrpcService : IGrpcService
    {
        private readonly ILogger<ChatGrpcService> _logger;
        private readonly ChatHub _chatHub;
        private readonly IPubSubHub _pubSubHub;
        private readonly IServerEventStreamsManager _serverEventStreamsManager;

        public ChatGrpcService(
            ILogger<ChatGrpcService> logger,
            ChatHub chatHub,
            IPubSubHub pubSubHub,
            IServerEventStreamsManager serverEventStreamsManager)
        {
            _logger = logger;
            _chatHub = chatHub;
            _pubSubHub = pubSubHub;
            _serverEventStreamsManager = serverEventStreamsManager;

            _pubSubHub.Subscribe<ChatSentMessage>(this, async message => await ChatSentMessageHandler(message));
        }


        private async Task ChatSentMessageHandler(ChatSentMessage chatSentMessage)
        {
            try
            {
                ServerEventRes serverEventRes = new ServerEventRes
                {
                    ServerEventType = ServerEventType.Chat,
                    CustomData = chatSentMessage.ChatMessage.ToJson()
                };
                await _serverEventStreamsManager.SendEventToPlayers(chatSentMessage.ChatRoomPlayersIds, serverEventRes);

                _logger.LogInformation("ChatSentMessageHandler, PlayersIds:{pis}, CustomData:{cd}", chatSentMessage.ChatRoomPlayersIds, serverEventRes.CustomData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public void AddRpcMethods(IGrpcBuilderAdapter grpcBuilder)
        {
            grpcBuilder.AddMethod(ChatGrpcDescriptors.JoinRoomDesc, JoinRoomRpc);
            grpcBuilder.AddMethod(ChatGrpcDescriptors.SendChatDesc, SendChatRPC);
        }


        public async Task<JoinRoomRes> JoinRoomRpc(JoinRoomReq req, ServerCallContext context)
        {
            int playerId = context.GetPlayerCredential().PlayerId;
            string roomName = req.RoomName;

            _chatHub.JoinRoom(playerId, roomName);
            _logger.LogInformation("JoinRoomRpc, PlayerId:{pi}, RoomName:{rn}", playerId, roomName);

            return new JoinRoomRes();
        }

        public async Task<SendChatRes> SendChatRPC(SendChatReq req, ServerCallContext context)
        {
            int playerId = context.GetPlayerCredential().PlayerId;
            string roomName = req.RoomName;
            string text = req.Text;

            _chatHub.SendChat(playerId, roomName, text);
            _logger.LogInformation("SendChatRPC, PlayerId:{pi}, RoomName:{rn}, Text:{txt}", playerId, roomName, text);

            return new SendChatRes();
        }
    }
}
