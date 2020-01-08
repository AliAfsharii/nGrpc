using nGrpc.Client;
using nGrpc.Client.GrpcServices;
using nGrpc.Common.Models;
using System;
using System.Threading.Tasks;
using Xunit;

namespace nGrpc.IntegrationTest.ClientTests
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
    }
}
