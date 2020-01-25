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
    public class MatchMaker
    {
        private readonly ISessionsManager _sessionsManager;
        private readonly IRoomCreator _roomCreator;

        IRoom _openRoom;
        int _lastRoomId = 0;
        AsyncLock _asyncLock = new AsyncLock();

        public MatchMaker(
            ISessionsManager sessionsManager,
            IRoomCreator roomCreator)
        {
            _sessionsManager = sessionsManager;
            _roomCreator = roomCreator;
        }


        // private



        // public

        public async Task<List<MatchMakePlayer>> MatchMake(int playerId)
        {
            using (await _asyncLock.LockAsync())
            {
                IRoom room;
                int roomId;
                if (_openRoom == null)
                {
                    room = _roomCreator.CreateRoom();
                    roomId = Interlocked.Increment(ref _lastRoomId);
                    _openRoom = room;
                }
                else
                    room = _openRoom;

                PlayerData playerData = _sessionsManager.GetPlayerData(playerId);
                (List<MatchMakePlayer> players, bool isRoomClosed) = await room.Join(playerId, playerData.Name);

                if (isRoomClosed == true)
                    _openRoom = null;

                return players;
            }
        }
    }
}
