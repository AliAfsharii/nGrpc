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

        [Fact]
        public async Task MatchMake_MatchMake_Two_Player_Test()
        {
            (GrpcChannel grpcChannel, LoginRes loginRes) = await TestUtils.GetNewLoginedChannel();
            MatchMakeGrpcService matchMakeGrpcService = new MatchMakeGrpcService(grpcChannel);
            MatchMakeUpdateEvent receivedEvent = null;
            matchMakeGrpcService.OnMatchMakeUpdateReceived += mEvent => receivedEvent = mEvent;
            MatchMakeReq req = new MatchMakeReq();
            MatchMakeRes res = await matchMakeGrpcService.MatchMakeRPC(req);

            (GrpcChannel grpcChannel1, LoginRes loginRes1) = await TestUtils.GetNewLoginedChannel();
            MatchMakeGrpcService matchMakeGrpcService1 = new MatchMakeGrpcService(grpcChannel1);
            MatchMakeReq req1 = new MatchMakeReq();
            MatchMakeRes res1 = await matchMakeGrpcService1.MatchMakeRPC(req1);

            Assert.NotNull(res);
            await Task.Delay(50);
            Assert.NotNull(receivedEvent);
            Assert.NotNull(receivedEvent.MatchId);
            Assert.Equal(2, receivedEvent.MatchMakePlayers.Count);
            Assert.Equal(loginRes.PlayerId, receivedEvent.MatchMakePlayers[0].Id);
            Assert.Equal(loginRes1.PlayerId, receivedEvent.MatchMakePlayers[1].Id);
        }
    }
}
