using nGrpc.Client;
using nGrpc.Client.GrpcServices;
using nGrpc.Common;
using System;
using System.Threading.Tasks;
using Xunit;

namespace nGrpc.IntegrationTest
{
    public class ProfileServiceTest : IntegrationTestBase
    {
        [Fact]
        public async Task Profile_RegisterRPC_Test()
        {
            GrpcChannel channel = await TestUtils.GetNewChannel();

            ProfileGrpcSerivce profileGrpcSerivce = new ProfileGrpcSerivce(channel);
            RegisterRes res = await profileGrpcSerivce.RegisterRPC(new RegisterReq());

            Assert.NotNull(res);
            Assert.True(res.PlayerId > 0);
            Assert.NotEqual(Guid.Empty, res.SecretKey);
        }

        [Fact]
        public async Task Profile_LoginRPC_Test()
        {
            GrpcChannel channel = await TestUtils.GetNewChannel();

            ProfileGrpcSerivce profileGrpcSerivce = new ProfileGrpcSerivce(channel);
            RegisterRes registerRes = await profileGrpcSerivce.RegisterRPC(new RegisterReq());
            LoginReq req = new LoginReq
            {
                PlayerId = registerRes.PlayerId,
                SecretKey = registerRes.SecretKey
            };
            LoginRes loginRes = await profileGrpcSerivce.LoginRPC(req);

            Assert.NotNull(loginRes);
            Assert.Equal(registerRes.PlayerId, loginRes.PlayerId);
            Assert.NotEqual(Guid.Empty, loginRes.SessionId);
        }

        [Fact]
        public async Task ChangeNameRPC_Test()
        {
            (GrpcChannel grpcChannel, LoginRes loginRes) = await TestUtils.GetNewLoginedChannel();
            ProfileGrpcSerivce profileGrpcSerivce = new ProfileGrpcSerivce(grpcChannel);

            ChangeNameReq req = new ChangeNameReq
            {
                NewName = "jahdsf sg asdoh asoihdfa dfa"
            };
            ChangeNameRes res = await profileGrpcSerivce.ChangeNameRPC(req);

            Assert.Equal(req.NewName, res.PlayerData.Name);
        }

        [Fact]
        public async Task GetPlayerDataRPC_Test()
        {
            (GrpcChannel grpcChannel, LoginRes loginRes) = await TestUtils.GetNewLoginedChannel();
            ProfileGrpcSerivce profileGrpcSerivce = new ProfileGrpcSerivce(grpcChannel);

            GetPlayerDataReq req = new GetPlayerDataReq();
            GetPlayerDataRes res = await profileGrpcSerivce.GetPlayerDataRPC(req);

            Assert.Equal(loginRes.PlayerId, res.PlayerData.Id);
        }
    }
}
