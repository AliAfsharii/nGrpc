using Grpc.Core;

namespace nGrpc.Common.Descriptors
{
    public static class ReversiGameGrpcDescriptors
    {
        public static readonly string ServiceName = "ReversiGameService";

        public static Method<GetGameDataReq, GetGameDataRes> GetGameDataDesc =
            GrpcUtils.CreateMethodDescriptor<GetGameDataReq, GetGameDataRes>(MethodType.Unary, ServiceName, "GetGameData");

        public static Method<PutDiskReq, PutDiskRes> PutDiskDesc =
            GrpcUtils.CreateMethodDescriptor<PutDiskReq, PutDiskRes>(MethodType.Unary, ServiceName, "PutDisk");
    }
}
