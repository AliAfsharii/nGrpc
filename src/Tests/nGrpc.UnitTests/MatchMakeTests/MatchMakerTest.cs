using nGrpc.Common;
using nGrpc.MatchMakeService;
using nGrpc.ServerCommon;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace nGrpc.UnitTests.MatchMakeTests
{
    public class MatchMakerTest
    {
        MatchMaker _matchMaker;
        ISessionsManager _sessionsManager;
        IRoomCreator _roomCreator;
        IMatchProvider _matchProvider;

        public MatchMakerTest()
        {
            _sessionsManager = Substitute.For<ISessionsManager>();
            _roomCreator = Substitute.For<IRoomCreator>();
            _matchProvider = Substitute.For<IMatchProvider>();
            _matchMaker = new MatchMaker(_sessionsManager, _roomCreator, _matchProvider);
        }


        [Fact]
        public async Task GIVEN_MatchMaker_WHEN_Call_MatchMake_THEN_It_Should_Call_CreateRoom_Once()
        {
            // given
            MatchMaker matchMaker = _matchMaker;

            int playerId = 73456;
            string playerName = "fdshfddf";
            ISessionsManager sessionsManager = _sessionsManager;
            sessionsManager.GetPlayerData(playerId).Returns(new PlayerData { Name = playerName });

            IRoomCreator roomCreator = _roomCreator;
            IRoom room = Substitute.For<IRoom>();
            List<MatchMakePlayer> mmplayers = new List<MatchMakePlayer>();
            room.Join(playerId, playerName).Returns((mmplayers, false));
            roomCreator.CreateRoom().Returns(room);

            // when
            (List<MatchMakePlayer> players, _) = await matchMaker.MatchMake(playerId);

            // then
            roomCreator.Received(1).CreateRoom();
            Assert.StrictEqual(mmplayers, players);
        }

        [Fact]
        public async Task GIVEN_MatchMaker_WHEN_Call_MatchMake_For_Two_Players_THEN_It_Should_Put_Players_To_TheSame_Room()
        {
            // given
            MatchMaker matchMaker = _matchMaker;

            ISessionsManager sessionsManager = _sessionsManager;
            int playerId1 = 73456;
            sessionsManager.GetPlayerData(playerId1).Returns(new PlayerData { });

            int playerId2 = 658456;
            sessionsManager.GetPlayerData(playerId2).Returns(new PlayerData { });

            IRoomCreator roomCreator = _roomCreator;
            IRoom room = Substitute.For<IRoom>();
            roomCreator.CreateRoom().Returns(room);

            // when
            await matchMaker.MatchMake(playerId1);
            await matchMaker.MatchMake(playerId2);

            // then
            roomCreator.Received(1).CreateRoom();
            room.Received(1).Join(playerId1, null);
            room.Received(1).Join(playerId2, null);
        }

        [Fact]
        public async Task GIVEN_MatchMaker_With_A_Player_WHEN_Call_MatchMake_For_Two_Players_THEN_It_Should_Put_Players_In_Different_Rooms()
        {
            // given
            MatchMaker matchMaker = _matchMaker;

            ISessionsManager sessionsManager = _sessionsManager;
            int playerId1 = 73456;
            sessionsManager.GetPlayerData(playerId1).Returns(new PlayerData { });

            int playerId2 = 658456;
            sessionsManager.GetPlayerData(playerId2).Returns(new PlayerData { });

            IRoomCreator roomCreator = _roomCreator;
            IRoom room1 = Substitute.For<IRoom>();
            room1.Join(playerId1, null).Returns((null, true));
            IRoom room2 = Substitute.For<IRoom>();
            int counter = 0;
            roomCreator.CreateRoom().Returns(x =>
            {
                counter++;
                return counter == 1 ? room1 : room2;
            });

            // when
            await matchMaker.MatchMake(playerId1);
            await matchMaker.MatchMake(playerId2);

            // then
            roomCreator.Received(2).CreateRoom();
            room1.Received(1).Join(playerId1, null);
            room2.Received(1).Join(playerId2, null);
        }

        [Fact]
        public async Task GIVEN_MatchMaker_WHEN_Call_MatchMake_And_Room_Get_Closed_THEN_It_Should_Return_A_MatchId()
        {
            // given
            MatchMaker matchMaker = _matchMaker;

            ISessionsManager sessionsManager = _sessionsManager;
            int playerId = 6846;
            sessionsManager.GetPlayerData(playerId).Returns(new PlayerData { });

            IRoomCreator roomCreator = _roomCreator;
            IRoom room = Substitute.For<IRoom>();
            room.Join(playerId, null).Returns((null, true));
            roomCreator.CreateRoom().Returns(room);

            IMatchProvider matchProvider = _matchProvider;
            int expectedMatchId = 783403423;
            matchProvider.CreateMatch(Arg.Any<List<int>>()).Returns(expectedMatchId);

            // when
            (_, int? matchId) = await matchMaker.MatchMake(playerId);

            // then
            Assert.Equal(expectedMatchId, matchId);
        }

        [Fact]
        public async Task GIVEN_MatchMaker_With_An_OpenRoom_WHEN_Call_Leave_THEN_Room_Leave_Should_Be_Called_Once()
        {
            // given
            MatchMaker matchMaker = _matchMaker;

            ISessionsManager sessionsManager = _sessionsManager;
            int playerId = 53732;
            sessionsManager.GetPlayerData(playerId).Returns(new PlayerData { });

            IRoomCreator roomCreator = _roomCreator;
            IRoom room = Substitute.For<IRoom>();
            List<MatchMakePlayer> expectedMatchMakePlayers = new List<MatchMakePlayer>();
            room.Leave(playerId).Returns(expectedMatchMakePlayers);
            roomCreator.CreateRoom().Returns(room);

            await matchMaker.MatchMake(playerId);

            // when
            List<MatchMakePlayer> players = await matchMaker.Leave(playerId);

            // then
            room.Received(1).Leave(playerId);
            Assert.StrictEqual(expectedMatchMakePlayers, players);
        }

        [Fact]
        public async Task GIVEN_MatchMaker_With_No_Room_WHEN_Call_Leave_THEN_It_Should_Throw_ThereIsNoOpenRoomException()
        {
            // given
            MatchMaker matchMaker = _matchMaker;
            int playerId = 34532;

            // when
            Exception exception = await Record.ExceptionAsync(() => matchMaker.Leave(playerId));

            // then
            Assert.NotNull(exception);
            Assert.IsType<ThereIsNoOpenRoomException>(exception);
        }
    }
}
