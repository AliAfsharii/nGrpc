using Grpc.Core;

namespace nGrpc.Common
{
    public static class BaseGrpcDescriptors
    {
        public static readonly string ServiceName = "BaseGrpcService";

        public static Method<ServerEventReq, ServerEventRes> ServerEventStreamDesc =
          GrpcUtils.CreateMethodDescriptor<ServerEventReq, ServerEventRes>(MethodType.DuplexStreaming, ServiceName, "ServerEventStream");

        public static Method<ServerEventTestReq, ServerEventTestRes> ServerEventTestDesc =
          GrpcUtils.CreateMethodDescriptor<ServerEventTestReq, ServerEventTestRes>(MethodType.Unary, ServiceName, "ServerEventTest"); 
    }
}
