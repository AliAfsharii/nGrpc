using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using nGrpc.Client;
using nGrpc.Client.GrpcServices;
using nGrpc.Common;
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

        public static async Task<(GrpcChannel grpcChannel, RegisterRes registerRes)> GetNewRegisteredChannel()
        {
            GrpcChannel grpcChannel = await GetNewChannel();
            var baseProfileService = new ProfileGrpcSerivce(grpcChannel);
            RegisterReq req = new RegisterReq();
            RegisterRes registerRes = await baseProfileService.RegisterRPC(req);
            return (grpcChannel, registerRes);
        }

        public static async Task<(GrpcChannel grpcChannel, LoginRes loginRes)> GetNewLoginedChannel()
        {
            var (grpcChannel, registerRes) = await GetNewRegisteredChannel();
            var baseProfileService = new ProfileGrpcSerivce(grpcChannel);
            LoginReq req = new LoginReq()
            {
                PlayerId = registerRes.PlayerId,
                SecretKey = registerRes.SecretKey
            };
            LoginRes loginRes = await baseProfileService.LoginRPC(req);
            return (grpcChannel, loginRes);
        }
    }
}
