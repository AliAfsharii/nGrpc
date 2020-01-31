using nGrpc.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nGrpc.ReversiGameService
{
    public interface IReversiGamesManager
    {
        Task<int> CreateMatch(List<MatchMakePlayer> players);
        ReversiGameData GetGameData(int playerId, int matchId);
        ReversiGameData PutDisk(int playerId, int matchId, int row, int col);
    }
}