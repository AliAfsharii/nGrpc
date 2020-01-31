using nGrpc.Common;
using nGrpc.Common.Descriptors;
using System.Threading.Tasks;

namespace nGrpc.Client.GrpcServices
{
    public class ReversiGameGrpcService
    {
        private readonly GrpcChannel _grpcChannel;

        public ReversiGameGrpcService(GrpcChannel grpcChannel)
        {
            _grpcChannel = grpcChannel;
        }

        public async Task<GetGameDataRes> GetGameDataRPC(GetGameDataReq req)
        {
            var res = await _grpcChannel.CallRpc(ReversiGameGrpcDescriptors.GetGameDataDesc, req);
            return res;
        }

        public async Task<PutDiskRes> PutDiskRPC(PutDiskReq req)
        {
            var res = await _grpcChannel.CallRpc(ReversiGameGrpcDescriptors.PutDiskDesc, req);
            return res;
        }
    }
}
