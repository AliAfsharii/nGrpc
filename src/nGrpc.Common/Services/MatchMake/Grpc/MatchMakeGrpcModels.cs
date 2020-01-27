using System.Collections.Generic;

namespace nGrpc.Common
{
    public class MatchMakeReq
    {

    }
    public class MatchMakeRes
    {

    }

    public class LeaveReq
    {

    }
    public class LeaveRes
    {

    }

    public class MatchMakeUpdateEvent
    {
        public List<MatchMakePlayer> MatchMakePlayers { get; set; }
        public int? MatchId { get; set; }
    }
}
