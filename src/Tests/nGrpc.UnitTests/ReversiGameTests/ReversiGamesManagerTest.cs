using nGrpc.Common;
using nGrpc.ReversiGameService;
using nGrpc.ServerCommon;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace nGrpc.UnitTests.ReversiGameTests
{
    public class ReversiGamesManagerTest
    {
        ReversiGamesManager _reversiGamesManager;
        IReversiLogicCreator _reversiLogicCreator;
        IReversiLogic _reversiLogic;
        int _playerId = 87456;

        public ReversiGamesManagerTest()
        {
            _reversiLogic = Substitute.For<IReversiLogic>();
            _reversiLogic.IsPlayerInGame(_playerId).Returns(true);

            _reversiLogicCreator = Substitute.For<IReversiLogicCreator>();
            _reversiLogicCreator.CreateLogic(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>()).Returns(_reversiLogic);

            _reversiGamesManager = new ReversiGamesManager(_reversiLogicCreator);
        }


        [Fact]
        public async Task GIVEN_ReversiGamesManager_WHEN_Call_CreateMatch_THEN_ReversiLogicCreator_CreateLogic_Should_Be_Called_Once()
        {
            // given
            ReversiGamesManager reversiGamesManager = _reversiGamesManager;
            IReversiLogicCreator reversiLogicCreator = _reversiLogicCreator;

            // when
            List<MatchMakePlayer> players = new List<MatchMakePlayer>
            {
                new MatchMakePlayer{ Id = 34534, Name = "346fdgsd" },
                new MatchMakePlayer{ Id = 34752, Name = "346ghdfhsdfdgsd" }
            };
            int matchId = await reversiGamesManager.CreateMatch(players);

            // then
            reversiLogicCreator.Received(1).CreateLogic(players[0].Id, players[0].Name, players[1].Id, players[1].Name);
        }

        [Fact]
        public async Task GIVEN_ReversiGameManager_With_A_CreatedGame_WHEN_Call_GetGameData_By_Given_MatchId_THEN_ReversiLogic_GetGameData_Should_Be_Called_Once()
        {
            // given
            ReversiGamesManager reversiGamesManager = _reversiGamesManager;
            int playerId = _playerId;
            List<MatchMakePlayer> players = new List<MatchMakePlayer>
            {
                new MatchMakePlayer{ Id = playerId, Name = "346fdgsd" },
                new MatchMakePlayer{ Id = 34752, Name = "346ghdfhsdfdgsd" }
            };
            int matchId = await reversiGamesManager.CreateMatch(players);

            IReversiLogic reversiLogic = _reversiLogic;
            ReversiGameData expectedGameData = new ReversiGameData();
            reversiLogic.GetGameData(_playerId).Returns(expectedGameData);

            // when
            ReversiGameData gameData = reversiGamesManager.GetGameData(playerId, matchId);

            // then
            reversiLogic.Received(1).GetGameData(playerId);
            Assert.StrictEqual(expectedGameData, gameData);
        }

        [Fact]
        public async Task GIVEN_ReversiGameManager_With_A_CreatedGame_WHEN_Call_GetGameData_By_Wrong_MatchId_THEN_It_Should_Throw_ThereIsNoSuchGameException()
        {
            // given
            ReversiGamesManager reversiGamesManager = _reversiGamesManager;
            int playerId = _playerId;
            List<MatchMakePlayer> players = new List<MatchMakePlayer>
            {
                new MatchMakePlayer{ Id = playerId, Name = "346fdgsd" },
                new MatchMakePlayer{ Id = 34752, Name = "346ghdfhsdfdgsd" }
            };
            int matchId = await reversiGamesManager.CreateMatch(players);

            // when
            Exception exception = Record.Exception(() => reversiGamesManager.GetGameData(playerId, matchId + 1));

            // then
            Assert.NotNull(exception);
            Assert.IsType<ThereIsNoSuchGameException>(exception);
        }

        [Fact]
        public async Task GIVEN_ReversiGamesManager_With_A_CreatedGame_WHEN_Call_PutDisk_THEN_()
        {
            // given
            ReversiGamesManager reversiGamesManager = _reversiGamesManager;
            int playerId = _playerId;
            List<MatchMakePlayer> players = new List<MatchMakePlayer>
            {
                new MatchMakePlayer{ Id = playerId, Name = "hddfg" },
                new MatchMakePlayer{ Id = 23452, Name = "fsdvcs" }
            };
            int matchId = await reversiGamesManager.CreateMatch(players);

            IReversiLogic reversiLogic = _reversiLogic;
            int row = 3;
            int col = 6;
            ReversiGameData expectedGameData = new ReversiGameData();
            reversiLogic.PutDisk(playerId, row, col).Returns(expectedGameData);

            // when
            ReversiGameData gameData = reversiGamesManager.PutDisk(playerId, matchId, row, col);

            // then
            reversiLogic.Received(1).PutDisk(playerId, row, col);
            Assert.StrictEqual(expectedGameData, gameData);
        }
    }
}
