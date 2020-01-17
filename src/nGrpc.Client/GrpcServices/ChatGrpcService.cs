using nGrpc.Common;
using nGrpc.Common.Descriptors;
using System;
using System.Threading.Tasks;

namespace nGrpc.Client.GrpcServices
{
    public class ChatGrpcService
    {
        public event Action<ChatMessage> OnChatReceived;

        private readonly GrpcChannel _grpcChannel;

        public ChatGrpcService(GrpcChannel channel)
        {
            _grpcChannel = channel;

            _grpcChannel.OnServerEventReceived += ServerEventReceivedCallback;
        }

        private void ServerEventReceivedCallback(ServerEventType serverEventType, string eventData)
        {
            if (serverEventType == ServerEventType.Chat)
            {
                ChatMessage chatMessage = eventData.ToObject<ChatMessage>();
                OnChatReceived?.Invoke(chatMessage);
            }
        }


        public async Task<JoinRoomRes> JoinRoomRPC(JoinRoomReq req)
        {
            var res = await _grpcChannel.CallRpc(ChatGrpcDescriptors.JoinRoomDesc, req);
            return res;
        }

        public async Task<SendChatRes> SendChatRPC(SendChatReq req)
        {
            var res = await _grpcChannel.CallRpc(ChatGrpcDescriptors.SendChatDesc, req);
            return res;
        }

        public async Task<LeaveRoomRes> LeaveRoomRPC(LeaveRoomReq req)
        {
            var res = await _grpcChannel.CallRpc(ChatGrpcDescriptors.LeaveRoomDesc, req);
            return res;
        }

        public async Task<GetLastChatsRes> GetLastChatsRPC(GetLastChatsReq req)
        {
            var res = await _grpcChannel.CallRpc(ChatGrpcDescriptors.GetLastChatsDesc, req);
            return res;
        }
    }
}
