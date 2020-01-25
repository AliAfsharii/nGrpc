using Microsoft.Extensions.Logging;
using nGrpc.Common;
using nGrpc.ServerCommon;
using Nito.AsyncEx;
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
        private readonly MatchMakeConfigs _matchMakeConfigs;
        private readonly IMatchProvider _matchProvider;

        class RoomPlayer
        {
            public int PositionInRoom { get; set; }
            public MatchMakePlayer MatchMakePlayer { get; set; }
        }

        ConcurrentDictionary<int, RoomPlayer> _joinedPlayers = new ConcurrentDictionary<int, RoomPlayer>();
        int _lastFilledPosition;
        bool _roomIsClosed = false;
        AsyncLock _asyncLock = new AsyncLock();


        public MatchMakeRoom(ILogger<MatchMakeRoom> logger,
            IPubSubHub pubSubHub,
            MatchMakeConfigs matchMakeConfigs,
            IMatchProvider matchProvider)
        {
            _logger = logger;
            _pubSubHub = pubSubHub;
            _matchMakeConfigs = matchMakeConfigs;
            _matchProvider = matchProvider;
        }



        // private

        private List<MatchMakePlayer> GetRoomPlayersInOrder()
        {
            List<MatchMakePlayer> matchMakePlayers = _joinedPlayers.Values
                .OrderBy(n => n.PositionInRoom)
                .Select(n => n.MatchMakePlayer)
                .ToList();

            return matchMakePlayers;
        }

        private bool IsPlayerInRoom(int playerId)
        {
            return _joinedPlayers.ContainsKey(playerId);
        }



        // public

        public async Task Join(int playerId, string playerName)
        {
            using (await _asyncLock.LockAsync())
            {
                if (IsPlayerInRoom(playerId) == true)
                    throw new PlayerIsAlreadyInRoomException($"PlayerId:{playerId}");

                if (_roomIsClosed == true)
                    throw new RoomIsClosedException();

                RoomPlayer roomPlayer = new RoomPlayer
                {
                    PositionInRoom = Interlocked.Increment(ref _lastFilledPosition),
                    MatchMakePlayer = new MatchMakePlayer
                    {
                        Id = playerId,
                        Name = playerName
                    }
                };

                _joinedPlayers.TryAdd(playerId, roomPlayer);

                if (_joinedPlayers.Count == _matchMakeConfigs.RoomCapacity)
                {
                    _roomIsClosed = true;

                    var closeMessage = new MatchMakeRoomClosedMessage
                    {
                        MatchId = await _matchProvider.CreateMatch(_joinedPlayers.Keys.ToList())
                    };
                    _pubSubHub.Publish(closeMessage);
                }

                var updateMessage = new MatchMakeRoomUpdatedMessage
                {
                    MatchMakePlayers = GetRoomPlayersInOrder()
                };
                _pubSubHub.Publish(updateMessage);
            }
        }
    }
}
