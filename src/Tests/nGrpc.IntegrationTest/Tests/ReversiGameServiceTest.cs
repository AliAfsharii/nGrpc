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

        [Fact]
        public async Task ReversiGame_Chat_Test()
        {
            (GrpcChannel grpcChannel, LoginRes loginRes) = await TestUtils.GetNewLoginedChannel();
            ReversiGameGrpcService reversiGameGrpcService = new ReversiGameGrpcService(grpcChannel);
            ChatGrpcService chatGrpcService = new ChatGrpcService(grpcChannel);
            ChatMessage receivedChatMessage = null;
            chatGrpcService.OnChatReceived += chatMessage => receivedChatMessage = chatMessage;

            (GrpcChannel grpcChannel1, LoginRes loginRes1) = await TestUtils.GetNewLoginedChannel();
            ReversiGameGrpcService reversiGameGrpcService1 = new ReversiGameGrpcService(grpcChannel1);
            ChatGrpcService chatGrpcService1 = new ChatGrpcService(grpcChannel1);
            ChatMessage receivedChatMessage1 = null;
            chatGrpcService1.OnChatReceived += chatMessage => receivedChatMessage1 = chatMessage;

            var manager = GetServiceFromProvider<nGrpc.ReversiGameService.IReversiGamesManager>();
            List<MatchMakePlayer> players = new List<MatchMakePlayer>
            {
                new MatchMakePlayer{Id = loginRes.PlayerId, Name = "p1"},
                new MatchMakePlayer{Id = loginRes1.PlayerId, Name = "p2"}
            };
            int gameId = await manager.CreateMatch(players);

            GetGameDataRes gameDataRes = await reversiGameGrpcService.GetGameDataRPC(new GetGameDataReq { GameId = gameId });
            string chatRoomName = gameDataRes.GameData.ChatRoomName;
            string text = "Hello From Player 0";
            await chatGrpcService.SendChatRPC(new SendChatReq { RoomName = chatRoomName, Text = text });

            await Task.Delay(50);
            Assert.NotNull(receivedChatMessage);
            Assert.Equal(text, receivedChatMessage.Text);
            Assert.NotNull(receivedChatMessage1);
            Assert.Equal(text, receivedChatMessage1.Text);

            receivedChatMessage = null;
            receivedChatMessage1 = null;

            string text1 = "Hello From Player 1";
            await chatGrpcService1.SendChatRPC(new SendChatReq { RoomName = chatRoomName, Text = text1 });

            await Task.Delay(50);
            Assert.NotNull(receivedChatMessage);
            Assert.Equal(text1, receivedChatMessage.Text);
            Assert.NotNull(receivedChatMessage1);
            Assert.Equal(text1, receivedChatMessage1.Text);
        }
    }
}
