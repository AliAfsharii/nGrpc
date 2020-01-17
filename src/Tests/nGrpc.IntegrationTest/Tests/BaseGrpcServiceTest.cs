using nGrpc.Client;
using nGrpc.Client.GrpcServices;
using nGrpc.Common;
using System.Threading.Tasks;
using Xunit;

namespace nGrpc.IntegrationTest
{
    public class BaseGrpcServiceTest : IntegrationTestBase
    {
        [Fact]
        public async Task GIVEN_BaseGrpcService_WHEN_Call_ServerEventTestRPC_THEN_It_Should_Send_A_TestServerEvent_To_Client()
        {
            (GrpcChannel grpcChannel, LoginRes loginRes) = await TestUtils.GetNewLoginedChannel();
            BaseGrpcService baseGrpcService = new BaseGrpcService(grpcChannel);
            string receivedCustomData = null;
            int eventCounter = 0;
            baseGrpcService.OnTestServerEventReceived += customData => { receivedCustomData = customData; eventCounter++; };

            ServerEventTestReq req = new ServerEventTestReq
            {
                CustomData = "hdskjlf uyds fhausdgf iudfa"
            };
            await baseGrpcService.ServerEventTestRPC(req);

            await Task.Delay(100);
            Assert.Equal(1, eventCounter);
            Assert.Equal(req.CustomData, receivedCustomData);
        }
    }
}
