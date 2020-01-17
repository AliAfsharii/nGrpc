using Grpc.Core;
using Microsoft.Extensions.Logging;
using nGrpc.Common;
using nGrpc.Common.Descriptors;
using nGrpc.ServerCommon;
using System.Threading.Tasks;

namespace nGrpc.ChatService
{
    public class ChatGrpcService : IGrpcService
    {
        private readonly ILogger<ChatGrpcService> _logger;
        private readonly ChatHub _chatHub;

        public ChatGrpcService(
            ILogger<ChatGrpcService> logger,
            ChatHub chatHub)
        {
            _logger = logger;
            _chatHub = chatHub;
        }

        public void AddRpcMethods(IGrpcBuilderAdapter grpcBuilder)
        {
            grpcBuilder.AddMethod(ChatGrpcDescriptors.JoinRoomDesc, JoinRoomRpc);
        }


        public async Task<JoinRoomRes> JoinRoomRpc(JoinRoomReq req, ServerCallContext context)
        {
            int playerId = context.GetPlayerCredential().PlayerId;
            string roomName = req.RoomName;

            _chatHub.JoinRoom(playerId, roomName);
            _logger.LogInformation("JoinRoomRpc, PlayerId:{pi}, RoomName:{rn}", playerId, roomName);

            return new JoinRoomRes();
        }
    }
}
