using nGrpc.Common;
using nGrpc.ServerCommon;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace nGrpc.ReversiGameService
{
    public class ReversiGamesManager : IMatchProvider, IReversiGamesManager
    {
        private readonly IReversiLogicCreator _reversiLogicCreator;
        private readonly ConcurrentDictionary<int, IReversiLogic> _logics = new ConcurrentDictionary<int, IReversiLogic>();
        private int _lastId = 0;

        public ReversiGamesManager(IReversiLogicCreator reversiLogicCreator)
        {
            _reversiLogicCreator = reversiLogicCreator;
        }


        // private

        private IReversiLogic GetReversiLogic(int playerId, int matchId)
        {
            if (_logics.TryGetValue(matchId, out IReversiLogic reversiLogic) == true)
                if (reversiLogic.IsPlayerInGame(playerId) == true)
                    return reversiLogic;

            throw new ThereIsNoSuchGameException($"PlayerId:{playerId}, MatchId:{matchId}");
        }


        //public

        public async Task<int> CreateMatch(List<MatchMakePlayer> players)
        {
            int playerId1 = players[0].Id;
            string playerName1 = players[0].Name;
            int playerId2 = players[1].Id;
            string playerName2 = players[1].Name;

            IReversiLogic reversiLogic = _reversiLogicCreator.CreateLogic(
                playerId1,
                playerName1,
                playerId2,
                playerName2
               );

            int newId = Interlocked.Increment(ref _lastId);
            _logics.TryAdd(newId, reversiLogic);

            return newId;
        }

        public ReversiGameData GetGameData(int playerId, int matchId)
        {
            return GetReversiLogic(playerId, matchId).GetGameData(playerId);
        }

        public ReversiGameData PutDisk(int playerId, int matchId, int row, int col)
        {
            IReversiLogic reversiLogic = GetReversiLogic(playerId, matchId);
            ReversiGameData gameData = reversiLogic.PutDisk(playerId, row, col);
            return gameData;
        }
    }
}
