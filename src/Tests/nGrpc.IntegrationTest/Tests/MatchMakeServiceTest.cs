using nGrpc.Client;
using nGrpc.Client.GrpcServices;
using nGrpc.Common;
using System.Threading.Tasks;
using Xunit;

namespace nGrpc.IntegrationTest
{
    public class MatchMakeServiceTest : IntegrationTestBase
    {
        [Fact]
        public async Task MatchMake_MatchMakeRPC_Test()
        {
            (GrpcChannel grpcChannel, LoginRes loginRes) = await TestUtils.GetNewLoginedChannel();
            MatchMakeGrpcService matchMakeGrpcService = new MatchMakeGrpcService(grpcChannel);

            MatchMakeUpdateEvent receivedEvent = null;
            matchMakeGrpcService.OnMatchMakeUpdateReceived += mEvent => receivedEvent = mEvent;

            MatchMakeReq req = new MatchMakeReq();
            MatchMakeRes res = await matchMakeGrpcService.MatchMakeRPC(req);

            Assert.NotNull(res);
            await Task.Delay(50);
            Assert.NotNull(receivedEvent);
            Assert.Null(receivedEvent.MatchId);
            Assert.Single(receivedEvent.MatchMakePlayers);
            Assert.Equal(loginRes.PlayerId, receivedEvent.MatchMakePlayers[0].Id);
        }

        [Fact]
        public async Task MatchMake_LeaveRPC_Test()
        {
            (GrpcChannel grpcChannel, LoginRes loginRes) = await TestUtils.GetNewLoginedChannel();
            MatchMakeGrpcService matchMakeGrpcService = new MatchMakeGrpcService(grpcChannel);

            MatchMakeReq r = new MatchMakeReq();
            await matchMakeGrpcService.MatchMakeRPC(r);

            LeaveReq req = new LeaveReq();
            LeaveRes res = await matchMakeGrpcService.LeaveRPC(req);

            Assert.NotNull(res);
        }
    }
}
