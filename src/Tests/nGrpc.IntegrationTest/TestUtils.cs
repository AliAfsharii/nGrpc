using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using nGrpc.Client;
using nGrpc.Grpc;
using nGrpc.Worker;

namespace nGrpc.IntegrationTest
{
    public static class TestUtils
    {
        public static async Task<GrpcChannel> GetNewChannel()
        {
            var grpcChannel = new GrpcChannel();
            GrpcConfigs grpcConfigs = Program.ServiceProviderForTests.GetRequiredService<GrpcConfigs>();
            await grpcChannel.Connect(grpcConfigs.Host, grpcConfigs.Port);
            return grpcChannel;
        }

        //public static async Task<(GrpcChannel grpcChannel, RegisterRes registerRes)> GetNewRegisteredChannel()
        //{
        //    GrpcChannel grpcChannel = await GetNewChannel();
        //    var baseProfileService = new BaseGrpcClientProxy.Services.BaseProfileGrpcService(grpcChannel);
        //    RegisterReq req = new RegisterReq()
        //    {
        //        ClientVersion = new Version(1, 2, 3, 0)
        //    };
        //    RegisterRes registerRes = await baseProfileService.RegisterRPC(req);
        //    return (grpcChannel, registerRes);
        //}

        //public static async Task<(GrpcChannel grpcChannel, LoginRes loginRes)> GetNewLoginedChannel()
        //{
        //    var (grpcChannel, registerRes) = await GetNewRegisteredChannel();
        //    var baseProfileService = new BaseGrpcClientProxy.Services.BaseProfileGrpcService(grpcChannel);
        //    LoginReq req = new LoginReq()
        //    {
        //        PlayerId = registerRes.PlayerId,
        //        UniqueKey = registerRes.UniqueKey,
        //        ClientVersion = new Version(5, 6, 7, 9)
        //    };
        //    LoginRes loginRes = await baseProfileService.LoginRPC(req);
        //    return (grpcChannel, loginRes);
        //}
    }
}
