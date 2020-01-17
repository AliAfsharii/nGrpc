using Grpc.Core;
using nGrpc.Client;
using nGrpc.Client.GrpcServices;
using nGrpc.Common;
using System;
using System.Threading.Tasks;
using Xunit;

namespace nGrpc.IntegrationTest
{
    public class GrpcAuthenticationTest : IntegrationTestBase
    {
        [Fact]
        public async Task GIVEN_ProfileGrpcSerive_With_UnAuthenticated_Channel_WHEN_Call_ChangeCustomData_THEN_It_Should_Throw_Exception()
        {
            GrpcChannel channel = await TestUtils.GetNewChannel();
            ProfileGrpcSerivce profileGrpcSerivce = new ProfileGrpcSerivce(channel);

            Exception exception = await Record.ExceptionAsync(() => profileGrpcSerivce.ChangeCustomDataRPC(new ChangeCustomDataReq { CustomData = "asdfa" }));

            Assert.NotNull(exception);
            Assert.IsType<RpcException>(exception);
            var rpcExcetpion = (RpcException)exception;
            Assert.Equal(StatusCode.InvalidArgument, rpcExcetpion.Status.StatusCode);
            Assert.Contains("Invalid Player Credentials", rpcExcetpion.Message);
        }
    }
}
