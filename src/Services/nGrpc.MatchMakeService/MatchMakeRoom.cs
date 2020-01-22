using Microsoft.Extensions.Logging;
using nGrpc.Common;
using nGrpc.ServerCommon;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nGrpc.MatchMakeService
{
    public class MatchMakeRoom
    {
        private readonly ILogger<MatchMakeRoom> _logger;
        private readonly IPubSubHub _pubSubHub;
        private readonly ISessionsManager _sessionsManager;

        public MatchMakeRoom(ILogger<MatchMakeRoom> logger,
            IPubSubHub pubSubHub,
            ISessionsManager sessionsManager)
        {
            _logger = logger;
            _pubSubHub = pubSubHub;
            _sessionsManager = sessionsManager;
        }

        public async Task Join(int playerId)
        {
            PlayerData playerData = _sessionsManager.GetPlayerData(playerId);
            var message = new MatchMakeRoomUpdatedMessage
            {
                MatchMakePlayers = new List<MatchMakePlayer>
                {
                    new MatchMakePlayer
                    {
                        Id = playerId,
                        Name = playerData.Name
                    }
                }
            };
            _pubSubHub.Publish(message);
        }
    }
}
