using System.Collections.Generic;

namespace nGrpc.MatchMakeService
{
    public class MatchMakeRoomUpdatedMessage
    {
        public List<MatchMakePlayer> MatchMakePlayers { get; set; }
    }
}
