using nGrpc.Client;
using nGrpc.Client.GrpcServices;
using nGrpc.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace nGrpc.IntegrationTest
{
    public class ReversiGameServiceTest : IntegrationTestBase
    {
        [Fact]
        public async Task ReversiGame_GetGameDataRPC_Test()
        {
            (GrpcChannel grpcChannel, LoginRes loginRes) = await TestUtils.GetNewLoginedChannel();
            (_, LoginRes loginRes1) = await TestUtils.GetNewLoginedChannel();
            ReversiGameGrpcService reversiGameGrpcService = new ReversiGameGrpcService(grpcChannel);

            var manager = GetServiceFromProvider<nGrpc.ReversiGameService.IReversiGamesManager>();
            List<MatchMakePlayer> players = new List<MatchMakePlayer>
            {
                new MatchMakePlayer{Id = loginRes.PlayerId, Name = "p1"},
                new MatchMakePlayer{Id = loginRes1.PlayerId, Name = "p2"}
            };
            int gameId = await manager.CreateMatch(players);

            GetGameDataRes res = await reversiGameGrpcService.GetGameDataRPC(new GetGameDataReq { GameId = gameId });

            Assert.NotNull(res);
            Assert.Equal(loginRes.PlayerId, res.GameData.PlayerId1);
            Assert.Equal(loginRes1.PlayerId, res.GameData.PlayerId2);
        }

        [Fact]
        public async Task ReversiGame_PutDiskRPC_Test()
        {
            (GrpcChannel grpcChannel, LoginRes loginRes) = await TestUtils.GetNewLoginedChannel();
            (_, LoginRes loginRes1) = await TestUtils.GetNewLoginedChannel();
            ReversiGameGrpcService reversiGameGrpcService = new ReversiGameGrpcService(grpcChannel);

            var manager = GetServiceFromProvider<nGrpc.ReversiGameService.IReversiGamesManager>();
            List<MatchMakePlayer> players = new List<MatchMakePlayer>
            {
                new MatchMakePlayer{Id = loginRes.PlayerId, Name = "p1"},
                new MatchMakePlayer{Id = loginRes1.PlayerId, Name = "p2"}
            };
            int gameId = await manager.CreateMatch(players);

            PutDiskReq req = new PutDiskReq
            {
                GameId = gameId,
                Row = 3,
                Col = 5
            };
            PutDiskRes res = await reversiGameGrpcService.PutDiskRPC(req);

            Assert.NotNull(res);
            Assert.Equal(loginRes.PlayerId, res.GameData.PlayerId1);
            Assert.Equal(loginRes1.PlayerId, res.GameData.PlayerId2);
            Assert.Equal(ReversiCellColor.White, res.GameData.CellColors[req.Row, req.Col]);
        }
    }
}
