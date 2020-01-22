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
        public async Task GIVEN_MatchMakeRoom_WHEN_Call_Join_THEN_PubSub_Publish_Should_Be_Called_Once()
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
    }
}
