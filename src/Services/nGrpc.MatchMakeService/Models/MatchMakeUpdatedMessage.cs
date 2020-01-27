using nGrpc.Common;
using System.Collections.Generic;

namespace nGrpc.MatchMakeService
{
    public class MatchMakeUpdatedMessage
    {
        public List<MatchMakePlayer> MatchMakePlayers { get; set; }
    }
}
