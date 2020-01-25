using System.Collections.Generic;

namespace nGrpc.MatchMakeService
{
    public interface IRoom
    {
        (List<MatchMakePlayer> players, bool isRoomClosed) Join(int playerId, string playerName);
        List<MatchMakePlayer> Leave(int playerId);
        List<int> GetPlayers();
    }
}