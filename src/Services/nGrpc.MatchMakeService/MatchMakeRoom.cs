using Microsoft.Extensions.Logging;
using nGrpc.Common;
using nGrpc.ServerCommon;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace nGrpc.MatchMakeService
{
    public class MatchMakeRoom
    {
        private readonly ILogger<MatchMakeRoom> _logger;
        private readonly IPubSubHub _pubSubHub;
        private readonly ISessionsManager _sessionsManager;
        private int _lastOrder;
        class RoomPlayer
        {
            public int Order { get; set; }
            public MatchMakePlayer MatchMakePlayer { get; set; }
        }

        ConcurrentDictionary<int, RoomPlayer> _joinedPlayers = new ConcurrentDictionary<int, RoomPlayer>();

        public MatchMakeRoom(ILogger<MatchMakeRoom> logger,
            IPubSubHub pubSubHub,
            ISessionsManager sessionsManager)
        {
            _logger = logger;
            _pubSubHub = pubSubHub;
            _sessionsManager = sessionsManager;
        }


        private List<MatchMakePlayer> GetRoomPlayersInOrder()
        {
            List<MatchMakePlayer> matchMakePlayers = _joinedPlayers.Values
                .OrderBy(n => n.Order)
                .Select(n => n.MatchMakePlayer)
                .ToList();

            return matchMakePlayers;
        }


        public async Task Join(int playerId)
        {
            PlayerData playerData = _sessionsManager.GetPlayerData(playerId);
            RoomPlayer roomPlayer = new RoomPlayer
            {
                Order = Interlocked.Increment(ref _lastOrder),
                MatchMakePlayer = new MatchMakePlayer
                {
                    Id = playerId,
                    Name = playerData.Name
                }
            };

            _joinedPlayers.TryAdd(playerId, roomPlayer);

            var message = new MatchMakeRoomUpdatedMessage
            {
                MatchMakePlayers = GetRoomPlayersInOrder()
            };
            _pubSubHub.Publish(message);
        }
    }
}
