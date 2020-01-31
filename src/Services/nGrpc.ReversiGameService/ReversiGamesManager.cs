using nGrpc.Common;
using nGrpc.ServerCommon;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace nGrpc.ReversiGameService
{
    public class ReversiGamesManager : IMatchProvider
    {
        private readonly ITimerProvider _timerProvider;
        private readonly ReversiGameConfigs _reversiGameConfigs;
        private readonly ConcurrentDictionary<int, ReversiLogic> _logics = new ConcurrentDictionary<int, ReversiLogic>();
        private int _lastId = 0;

        public ReversiGamesManager(
            ITimerProvider timerProvider,
            ReversiGameConfigs reversiGameConfigs)
        {
            _timerProvider = timerProvider;
            _reversiGameConfigs = reversiGameConfigs;
        }

        public async Task<int> CreateMatch(List<MatchMakePlayer> players)
        {
            ITimer timer = _timerProvider.CreateTimer();
            int playerId1 = players[0].Id;
            string playerName1 = players[0].Name;
            int playerId2 = players[1].Id;
            string playerName2 = players[1].Name;

            ReversiLogic reversiLogic = new ReversiLogic(
                _reversiGameConfigs,
                timer,
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
            if (_logics.TryGetValue(matchId, out ReversiLogic reversiLogic) == true)
                return reversiLogic.GetGameData(playerId);

            throw new ThereIsNoSuchGameException($"PlayerId:{playerId}, MatchId:{matchId}");
        }
    }
}
