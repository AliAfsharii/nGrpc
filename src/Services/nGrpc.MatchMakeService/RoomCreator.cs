using System;

namespace nGrpc.MatchMakeService
{
    public class RoomCreator : IRoomCreator
    {
        private readonly MatchMakeConfigs _matchMakeConfigs;

        public RoomCreator(MatchMakeConfigs matchMakeConfigs)
        {
            _matchMakeConfigs = matchMakeConfigs;
        }

        public IRoom CreateRoom()
        {
            Room room = new Room(_matchMakeConfigs);
            return room;
        }
    }
}
