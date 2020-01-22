using Microsoft.Extensions.Logging;
using nGrpc.Common;
using nGrpc.MatchMakeService;
using nGrpc.ServerCommon;
using NSubstitute;
using System.Threading.Tasks;
using Xunit;

namespace nGrpc.UnitTests.MatchMakeTests
{
    public class MatchMakeRoomTest
    {
        MatchMakeRoom _matchMakeRoom;
        IPubSubHub _pubSubHub;
        ISessionsManager _sessionsManager;

        public MatchMakeRoomTest()
        {
            ILogger<MatchMakeRoom> logger = Substitute.For<ILogger<MatchMakeRoom>>();
            _pubSubHub = Substitute.For<IPubSubHub>();
            _sessionsManager = Substitute.For<ISessionsManager>();
            _matchMakeRoom = new MatchMakeRoom(logger, _pubSubHub, _sessionsManager);
        }

        [Fact]
        public async Task GIVEN_MatchMakeRoom_WHEN_Call_Join_THEN_PubSub_Publish_Should_Be_Called_Once_With_PlayerData()
        {
            MatchMakeRoom matchMakeRoom = _matchMakeRoom;
            int playerId = 340975;
            string playerName = "ghdfwr";
            IPubSubHub pubSubHub = _pubSubHub;
            ISessionsManager sessionsManager = _sessionsManager;
            _sessionsManager.GetPlayerData(playerId).Returns(new PlayerData { Id = playerId, Name = playerName });

            await matchMakeRoom.Join(playerId);

            pubSubHub.Received(1).Publish(Arg.Is<MatchMakeRoomUpdatedMessage>(message =>
                message.MatchMakePlayers[0].Id == playerId
                && message.MatchMakePlayers[0].Name == playerName));
        }

        [Fact]
        public async Task GIVEN_MatchMakeRoom_WHEN_Call_Join_With_Two_Different_PlayerId_THEN_PubSub_Publish_Should_Be_Called_Once_With_Two_PlayerData()
        {
            MatchMakeRoom matchMakeRoom = _matchMakeRoom;
            int playerId1 = 340975;
            string playerName1 = "ghdfwr";
            int playerId2 = 45632;
            string playerName2 = "dfgdsdfgssf";
            IPubSubHub pubSubHub = _pubSubHub;
            ISessionsManager sessionsManager = _sessionsManager;
            _sessionsManager.GetPlayerData(playerId1).Returns(new PlayerData { Id = playerId1, Name = playerName1 });
            _sessionsManager.GetPlayerData(playerId2).Returns(new PlayerData { Id = playerId2, Name = playerName2 });

            await matchMakeRoom.Join(playerId1);
            await matchMakeRoom.Join(playerId2);

            pubSubHub.Received(1).Publish(Arg.Is<MatchMakeRoomUpdatedMessage>(message =>
                message.MatchMakePlayers[0].Id == playerId1
                && message.MatchMakePlayers[0].Name == playerName1
                && message.MatchMakePlayers[1].Id == playerId2
                && message.MatchMakePlayers[1].Name == playerName2));
        }
    }
}
