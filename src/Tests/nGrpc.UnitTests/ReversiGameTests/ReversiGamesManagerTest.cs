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

        public ReversiGamesManagerTest()
        {
            ITimerProvider timerProvider = Substitute.For<ITimerProvider>();
            _reversiGamesManager = new ReversiGamesManager(timerProvider, new ReversiGameConfigs());
        }


        [Fact]
        public async Task GIVEN_ReversiGamesManager_WHEN_Call_CreateMatch_THEN_It_Should_Return_MatchId()
        {
            // given
            ReversiGamesManager reversiGamesManager = _reversiGamesManager;

            // when
            List<MatchMakePlayer> players = new List<MatchMakePlayer>
            {
                new MatchMakePlayer{ Id = 34534, Name = "346fdgsd" },
                new MatchMakePlayer{ Id = 34752, Name = "346ghdfhsdfdgsd" }
            };
            int matchId = await reversiGamesManager.CreateMatch(players);

            // then
            Assert.NotEqual(0, matchId);
        }

        [Fact]
        public async Task GIVEN_ReversiGameManager_With_A_CreatedGame_WHEN_Call_GetGameData_By_Given_MatchId_THEN_It_Should_Return_Correct_GameData()
        {
            // given
            ReversiGamesManager reversiGamesManager = _reversiGamesManager;
            int playerId = 9875634;
            List<MatchMakePlayer> players = new List<MatchMakePlayer>
            {
                new MatchMakePlayer{ Id = playerId, Name = "346fdgsd" },
                new MatchMakePlayer{ Id = 34752, Name = "346ghdfhsdfdgsd" }
            };
            int matchId = await reversiGamesManager.CreateMatch(players);

            // when
            ReversiGameData reversiGameData = reversiGamesManager.GetGameData(playerId, matchId);

            // then
            Assert.Equal(playerId, reversiGameData.PlayerId1);
        }

        [Fact]
        public async Task GIVEN_ReversiGameManager_With_A_CreatedGame_WHEN_Call_GetGameData_By_Wrong_MatchId_THEN_It_Should_Throw_ThereIsNoSuchGameException()
        {
            // given
            ReversiGamesManager reversiGamesManager = _reversiGamesManager;
            int playerId = 9875634;
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
        public async Task GIVEN_ReversiGameManager_With_A_CreatedGame_WHEN_Call_GetGameData_By_Wrong_PlayerId_THEN_It_Should_Throw_WrongPlayerIdException()
        {
            // given
            ReversiGamesManager reversiGamesManager = _reversiGamesManager;
            int playerId = 9875634;
            List<MatchMakePlayer> players = new List<MatchMakePlayer>
            {
                new MatchMakePlayer{ Id = playerId, Name = "346fdgsd" },
                new MatchMakePlayer{ Id = 34752, Name = "346ghdfhsdfdgsd" }
            };
            int matchId = await reversiGamesManager.CreateMatch(players);

            // when
            Exception exception = Record.Exception(() => reversiGamesManager.GetGameData(playerId + 1, matchId));

            // then
            Assert.NotNull(exception);
            Assert.IsType<WrongPlayerIdException>(exception);
        }

    }
}
