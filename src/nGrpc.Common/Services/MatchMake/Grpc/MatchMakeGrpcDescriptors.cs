using Grpc.Core;

namespace nGrpc.Common.Descriptors
{
    public static class MatchMakeGrpcDescriptors
    {
        public static readonly string ServiceName = "MatchMakeService";

        public static Method<MatchMakeReq, MatchMakeRes> MatchMakeDesc =
            GrpcUtils.CreateMethodDescriptor<MatchMakeReq, MatchMakeRes>(MethodType.Unary, ServiceName, "MatchMake");

        public static Method<LeaveReq, LeaveRes> LeaveDesc =
            GrpcUtils.CreateMethodDescriptor<LeaveReq, LeaveRes>(MethodType.Unary, ServiceName, "Leave");
    }
}
