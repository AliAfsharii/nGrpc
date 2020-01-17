using nGrpc.Common;
using nGrpc.Common.Descriptors;
using System.Threading.Tasks;

namespace nGrpc.Client.GrpcServices
{
    public class ChatGrpcService
    {
        private readonly GrpcChannel _grpcChannel;

        public ChatGrpcService(GrpcChannel channel)
        {
            _grpcChannel = channel;
        }


        public async Task<JoinRoomRes> JoinRoomRPC(JoinRoomReq req)
        {
            var res = await _grpcChannel.CallRpc(ChatGrpcDescriptors.JoinRoomDesc, req);
            return res;
        }
    }
}
