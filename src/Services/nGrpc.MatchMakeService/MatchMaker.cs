using nGrpc.Common;
using nGrpc.ServerCommon;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace nGrpc.MatchMakeService
{
    public class MatchMaker
    {
        private MatchMakeConfigs _matchMakeConfigs;
        private readonly ISessionsManager _sessionsManager;
        private readonly IRoomCreator _roomCreator;

        ConcurrentDictionary<int, IRoom> _rooms = new ConcurrentDictionary<int, IRoom>();
        int _lastRoomId = 0;

        public MatchMaker(MatchMakeConfigs matchMakeConfigs,
            ISessionsManager sessionsManager,
            IRoomCreator roomCreator)
        {
            _matchMakeConfigs = matchMakeConfigs;
            _sessionsManager = sessionsManager;
            _roomCreator = roomCreator;
        }


        // private



        // public

        public async Task<List<MatchMakePlayer>> MatchMake(int playerId)
        {
            IRoom room;
            if (_rooms.Count == 0)
            {
                room = _roomCreator.CreateRoom();
                int roomId = Interlocked.Increment(ref _lastRoomId);
                _rooms.TryAdd(roomId, room);
            }
            else
                room = _rooms.Values.First();

            PlayerData playerData = _sessionsManager.GetPlayerData(playerId);
            (List<MatchMakePlayer> players, bool isRoomClosed) = await room.Join(playerId, playerData.Name);
            return players;
        }
    }
}
