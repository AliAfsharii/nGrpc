using Grpc.Core;

namespace nGrpc.Common.Descriptors
{
    public static class ProfileDescriptors
    {
        public static readonly string ServiceName = "ProfileService";

        public static Method<RegisterReq, RegisterRes> RegisterDesc =
                      GrpcUtils.CreateMethodDescriptor<RegisterReq, RegisterRes>(MethodType.Unary, ServiceName, "Register");

        public static Method<LoginReq, LoginRes> LoginDesc =
             GrpcUtils.CreateMethodDescriptor<LoginReq, LoginRes>(MethodType.Unary, ServiceName, "Login");

        public static Method<ChangeCustomDataReq, ChangeCustomDataRes> ChangeCustomDataDesc =
        GrpcUtils.CreateMethodDescriptor<ChangeCustomDataReq, ChangeCustomDataRes>(MethodType.Unary, ServiceName, "ChangeCustomData");
    }
}
