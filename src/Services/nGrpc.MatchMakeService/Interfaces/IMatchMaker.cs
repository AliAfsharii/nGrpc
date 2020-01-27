using nGrpc.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nGrpc.MatchMakeService
{
    public interface IMatchMaker
    {
        Task<List<MatchMakePlayer>> Leave(int playerId);
        Task<(List<MatchMakePlayer>, int? matchId)> MatchMake(int playerId);
    }
}