using System.Collections.Generic;
using System.Threading.Tasks;

namespace nGrpc.MatchMakeService
{
    public interface IRoom
    {
        Task<(List<MatchMakePlayer> players, bool isRoomClosed)> Join(int playerId, string playerName);
        Task<List<MatchMakePlayer>> Leave(int playerId);
    }
}