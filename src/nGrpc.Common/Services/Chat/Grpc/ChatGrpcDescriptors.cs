using Grpc.Core;

namespace nGrpc.Common.Descriptors
{
    public static class ChatGrpcDescriptors
    {
        public static readonly string ServiceName = "ChatService";

        public static Method<JoinRoomReq, JoinRoomRes> JoinRoomDesc =
            GrpcUtils.CreateMethodDescriptor<JoinRoomReq, JoinRoomRes>(MethodType.Unary, ServiceName, "JoinRoom");

        public static Method<SendChatReq, SendChatRes> SendChatDesc =
           GrpcUtils.CreateMethodDescriptor<SendChatReq, SendChatRes>(MethodType.Unary, ServiceName, "SendChat");

    }
}
