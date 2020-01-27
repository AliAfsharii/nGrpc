using System;
using System.Threading.Tasks;
using nGrpc.Common;
using nGrpc.Common.Descriptors;

namespace nGrpc.Client.GrpcServices
{
    public class MatchMakeGrpcService
    {
        public event Action<MatchMakeUpdateEvent> OnMatchMakeUpdateReceived;

        private readonly GrpcChannel _grpcChannel;

        public MatchMakeGrpcService(GrpcChannel channel)
        {
            _grpcChannel = channel;
            _grpcChannel.OnServerEventReceived += ServerEventReceivedCallback;
        }

        private void ServerEventReceivedCallback(ServerEventType serverEventType, string eventData)
        {
            if (serverEventType == ServerEventType.MatchMake)
            {
                MatchMakeUpdateEvent matchMakeUpdateEvent = eventData.ToObject<MatchMakeUpdateEvent>();
                OnMatchMakeUpdateReceived?.Invoke(matchMakeUpdateEvent);
            }
        }

        public async Task<MatchMakeRes> MatchMakeRPC(MatchMakeReq req)
        {
            var res = await _grpcChannel.CallRpc(MatchMakeGrpcDescriptors.MatchMakeDesc, req);
            return res;
        }

        public async Task<LeaveRes> LeaveRPC(LeaveReq req)
        {
            var res = await _grpcChannel.CallRpc(MatchMakeGrpcDescriptors.LeaveDesc, req);
            return res;
        }
    }
}
