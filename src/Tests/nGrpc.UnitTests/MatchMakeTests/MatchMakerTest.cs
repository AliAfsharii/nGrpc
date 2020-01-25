using nGrpc.Common;
using nGrpc.MatchMakeService;
using nGrpc.ServerCommon;
using NSubstitute;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace nGrpc.UnitTests.MatchMakeTests
{
    public class MatchMakerTest
    {
        MatchMaker _matchMaker;
        MatchMakeConfigs _matchMakeConfigs = new MatchMakeConfigs { RoomCapacity = 2 };
        ISessionsManager _sessionsManager;
        IRoomCreator _roomCreator;

        public MatchMakerTest()
        {
            _sessionsManager = Substitute.For<ISessionsManager>();
            _roomCreator = Substitute.For<IRoomCreator>();
            _matchMaker = new MatchMaker(_matchMakeConfigs, _sessionsManager, _roomCreator);
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
            List<MatchMakePlayer> players = await matchMaker.MatchMake(playerId);

            // then
            roomCreator.Received(1).CreateRoom();
            Assert.StrictEqual(mmplayers, players);
        }

        [Fact]
        public async Task GIVEN_MatchMaker_With_One_Player_WHEN_Call_MatchMake_With_Another_Player_THEN_It_Should_Put_First_And_Second_Players_To_TheSame_Room()
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
            await room.Received(1).Join(playerId1, null);
            await room.Received(1).Join(playerId2, null);
        }
    }
}
