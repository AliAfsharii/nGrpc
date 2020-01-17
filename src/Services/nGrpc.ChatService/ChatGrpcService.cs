using Grpc.Core;
using Microsoft.Extensions.Logging;
using nGrpc.Common;
using nGrpc.Common.Descriptors;
using nGrpc.ServerCommon;
using System;
using System.Collections.Generic;
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
            grpcBuilder.AddMethod(ChatGrpcDescriptors.JoinRoomDesc, JoinRoomRPC);
            grpcBuilder.AddMethod(ChatGrpcDescriptors.SendChatDesc, SendChatRPC);
            grpcBuilder.AddMethod(ChatGrpcDescriptors.LeaveRoomDesc, LeaveRoomRPC);
            grpcBuilder.AddMethod(ChatGrpcDescriptors.GetLastChatsDesc, GetLastChatsRPC);
        }


        public async Task<JoinRoomRes> JoinRoomRPC(JoinRoomReq req, ServerCallContext context)
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

        public async Task<LeaveRoomRes> LeaveRoomRPC(LeaveRoomReq req, ServerCallContext context)
        {
            int playerId = context.GetPlayerCredential().PlayerId;
            string roomName = req.RoomName;

            _chatHub.LeaveRoom(playerId, roomName);
            _logger.LogInformation("LeaveRoomRPC, PlayerId:{pi}, RoomName:{rn}", playerId, roomName);

            return new LeaveRoomRes();
        }

        public async Task<GetLastChatsRes> GetLastChatsRPC(GetLastChatsReq req, ServerCallContext context)
        {
            int playerId = context.GetPlayerCredential().PlayerId;
            string roomName = req.RoomName;
            int lastChatId = req.LastChatId;

            List<ChatMessage> chatMessages = _chatHub.GetLastChats(playerId, roomName, lastChatId);
            _logger.LogInformation("GetLastChatsRPC, PlayerId:{pi}, RoomName:{rn}, LastChatId:{lci}", playerId, roomName, lastChatId);

            return new GetLastChatsRes
            {
                ChatMessages = chatMessages
            };
        }
    }
}
