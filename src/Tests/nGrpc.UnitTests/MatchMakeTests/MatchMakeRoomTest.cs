using Microsoft.Extensions.Logging;
using nGrpc.Common;
using nGrpc.MatchMakeService;
using nGrpc.ServerCommon;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace nGrpc.UnitTests.MatchMakeTests
{
    public class MatchMakeRoomTest
    {
        MatchMakeRoom _matchMakeRoom;
        IPubSubHub _pubSubHub;
        MatchMakeConfigs _matchMakeConfigs = new MatchMakeConfigs();
        IMatchProvider _matchProvider;

        public MatchMakeRoomTest()
        {
            ILogger<MatchMakeRoom> logger = Substitute.For<ILogger<MatchMakeRoom>>();
            _pubSubHub = Substitute.For<IPubSubHub>();
            _matchProvider = Substitute.For<IMatchProvider>();

            _matchMakeRoom = new MatchMakeRoom(logger, _pubSubHub, _matchMakeConfigs, _matchProvider);
        }

        [Fact]
        public async Task GIVEN_MatchMakeRoom_WHEN_Call_Join_THEN_PubSub_Publish_Should_Be_Called_Once_With_PlayerData()
        {
            // given
            MatchMakeRoom matchMakeRoom = _matchMakeRoom;
            int playerId = 68464;
            string playerName = "asdfhsdsadf";
            IPubSubHub pubSubHub = _pubSubHub;

            // when
            await matchMakeRoom.Join(playerId, playerName);

            // then
            pubSubHub.Received(1).Publish(Arg.Is<MatchMakeRoomUpdatedMessage>(message =>
                message.MatchMakePlayers[0].Id == playerId
                && message.MatchMakePlayers[0].Name == playerName));
        }

        [Fact]
        public async Task GIVEN_MatchMakeRoom_WHEN_Call_Join_With_Two_Different_PlayerId_THEN_PubSub_Publish_Should_Be_Called_Once_With_Two_PlayerData()
        {
            // given
            MatchMakeRoom matchMakeRoom = _matchMakeRoom;
            int playerId1 = 564;
            string playerName1 = "ghdfwr";
            int playerId2 = 45632;
            string playerName2 = "dfgdsdfgssf";
            IPubSubHub pubSubHub = _pubSubHub;

            // when
            await matchMakeRoom.Join(playerId1, playerName1);
            await matchMakeRoom.Join(playerId2, playerName2);

            // then
            pubSubHub.Received(1).Publish(Arg.Is<MatchMakeRoomUpdatedMessage>(message =>
                message.MatchMakePlayers[0].Id == playerId1
                && message.MatchMakePlayers[0].Name == playerName1
                && message.MatchMakePlayers[1].Id == playerId2
                && message.MatchMakePlayers[1].Name == playerName2));
        }

        [Fact]
        public async Task GIVEN_MatchMakeRoom_With_A_Joined_Player_WHEN_Call_Join_With_TheSame_Player_THEN_It_Should_Throw_PlayerIsAlreadyInRoomException()
        {
            // given
            MatchMakeRoom matchMakeRoom = _matchMakeRoom;
            int playerId = 7846;
            await matchMakeRoom.Join(playerId, "");

            // when
            Exception exception = await Record.ExceptionAsync(() => matchMakeRoom.Join(playerId, ""));

            // then
            Assert.NotNull(exception);
            Assert.IsType<PlayerIsAlreadyInRoomException>(exception);
        }

        [Fact]
        public async Task GIVEN_MatchMakeRoom_WHEN_Make_Room_Full_THEN_PubSub_Publish_Should_Be_Called_Once_With_Correct_Message()
        {
            // given
            MatchMakeRoom matchMakeRoom = _matchMakeRoom;
            int playerId = 345;
            MatchMakeConfigs matchMakeConfigs = _matchMakeConfigs;
            matchMakeConfigs.RoomCapacity = 2;
            IPubSubHub pubSubHub = _pubSubHub;

            int matchId = 984658743;
            IMatchProvider matchProvider = _matchProvider;
            List<int> playerIds = Enumerable.Range(0, matchMakeConfigs.RoomCapacity).Select(n => playerId + n).ToList();
            matchProvider.CreateMatch(Arg.Is<List<int>>(l => l.ToJson() == playerIds.ToJson())).Returns(matchId);

            // when
            for (int i = 0; i < matchMakeConfigs.RoomCapacity; i++)
                await matchMakeRoom.Join(playerId + i, "");

            // then
            pubSubHub.Received(1).Publish(Arg.Is<MatchMakeRoomClosedMessage>(message => message.MatchId == matchId));
        }

        [Fact]
        public async Task GIVEN_MatchMakeRoom_Full_Of_Players_WHEN_Call_Join_With_New_PlayerId_THEN_It_Should_Throw_RoomIsClosedException()
        {
            // given
            MatchMakeRoom matchMakeRoom = _matchMakeRoom;
            int playerId = 6584;
            MatchMakeConfigs matchMakeConfigs = _matchMakeConfigs;
            matchMakeConfigs.RoomCapacity = 2;
            for (int i = 0; i < matchMakeConfigs.RoomCapacity; i++)
                await matchMakeRoom.Join(playerId + i, "");

            // when
            Exception exception = await Record.ExceptionAsync(() => matchMakeRoom.Join(playerId + 100, ""));

            // then
            Assert.NotNull(exception);
            Assert.IsType<RoomIsClosedException>(exception);
        }
    }
}
