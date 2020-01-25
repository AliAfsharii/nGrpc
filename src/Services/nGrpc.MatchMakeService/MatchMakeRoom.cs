using Microsoft.Extensions.Logging;
using nGrpc.ServerCommon;
using Nito.AsyncEx;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace nGrpc.MatchMakeService
{
    public class MatchMakeRoom
    {
        private readonly MatchMakeConfigs _matchMakeConfigs;

        class RoomPlayer
        {
            public int PositionInRoom { get; set; }
            public MatchMakePlayer MatchMakePlayer { get; set; }
        }

        ConcurrentDictionary<int, RoomPlayer> _joinedPlayers = new ConcurrentDictionary<int, RoomPlayer>();
        int _lastFilledPosition;
        bool _roomIsClosed = false;
        AsyncLock _asyncLock = new AsyncLock();


        public MatchMakeRoom(
            MatchMakeConfigs matchMakeConfigs)
        {
            _matchMakeConfigs = matchMakeConfigs;
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

        private RoomPlayer CreateRoomPlayer(int playerId, string playerName)
        {
            return new RoomPlayer
            {
                PositionInRoom = Interlocked.Increment(ref _lastFilledPosition),
                MatchMakePlayer = new MatchMakePlayer
                {
                    Id = playerId,
                    Name = playerName
                }
            };
        }

        //private void PublishRoomClosedMessage(int matchId)
        //{
        //    var closeMessage = new MatchMakeRoomClosedMessage
        //    {
        //        MatchId = matchId
        //    };
        //    _pubSubHub.Publish(closeMessage);
        //}

        //private void PublishRoomUpdatedMessage()
        //{
        //    var updateMessage = new MatchMakeRoomUpdatedMessage
        //    {
        //        MatchMakePlayers = GetRoomPlayersInOrder()
        //    };
        //    _pubSubHub.Publish(updateMessage);
        //}


        // public

        public async Task<(List<MatchMakePlayer> players, bool isRoomClosed)> Join(int playerId, string playerName)
        {
            using (await _asyncLock.LockAsync())
            {
                if (IsPlayerInRoom(playerId) == true)
                    throw new PlayerIsAlreadyInRoomException($"PlayerId:{playerId}");
                if (_roomIsClosed == true)
                    throw new RoomIsClosedException();

                RoomPlayer roomPlayer = CreateRoomPlayer(playerId, playerName);
                _joinedPlayers.TryAdd(playerId, roomPlayer);

                if (_joinedPlayers.Count == _matchMakeConfigs.RoomCapacity)
                {
                    _roomIsClosed = true;
                    // int matchId = await _matchProvider.CreateMatch(_joinedPlayers.Keys.ToList());
                    //PublishRoomClosedMessage(matchId);
                }

                //PublishRoomUpdatedMessage();
                return (GetRoomPlayersInOrder(), _roomIsClosed);
            }
        }

        public async Task<List<MatchMakePlayer>> Leave(int playerId)
        {
            using (await _asyncLock.LockAsync())
            {
                if (IsPlayerInRoom(playerId) == false)
                    throw new PlayerIsNotInRoomException($"PlayerId:{playerId}");

                _joinedPlayers.TryRemove(playerId, out _);

                return GetRoomPlayersInOrder();
            }
        }
    }
}
