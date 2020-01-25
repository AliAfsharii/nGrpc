using nGrpc.Common;
using nGrpc.ServerCommon;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nGrpc.MatchMakeService
{
    public class MatchMaker
    {
        private readonly ISessionsManager _sessionsManager;
        private readonly IRoomCreator _roomCreator;
        private readonly IMatchProvider _matchProvider;

        IRoom _openRoom;
        AsyncLock _asyncLock = new AsyncLock();

        public MatchMaker(
            ISessionsManager sessionsManager,
            IRoomCreator roomCreator,
            IMatchProvider matchProvider)
        {
            _sessionsManager = sessionsManager;
            _roomCreator = roomCreator;
            _matchProvider = matchProvider;
        }


        // private



        // public

        public async Task<(List<MatchMakePlayer>, int? matchId)> MatchMake(int playerId)
        {
            using (await _asyncLock.LockAsync())
            {
                IRoom room;
                if (_openRoom == null)
                {
                    room = _roomCreator.CreateRoom();
                    _openRoom = room;
                }
                else
                    room = _openRoom;

                PlayerData playerData = _sessionsManager.GetPlayerData(playerId);
                (List<MatchMakePlayer> players, bool isRoomClosed) = room.Join(playerId, playerData.Name);

                int? matchId = null;
                if (isRoomClosed == true)
                {
                    _openRoom = null;
                    matchId = await _matchProvider.CreateMatch(room.GetPlayers());
                }

                return (players, matchId);
            }
        }

        public async Task<List<MatchMakePlayer>> Leave(int playerId)
        {
            using(await _asyncLock.LockAsync())
            {
                List<MatchMakePlayer> matchMakePlayers = _openRoom?.Leave(playerId);
                return matchMakePlayers;
            }
        }
    }
}
