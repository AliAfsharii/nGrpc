using nGrpc.ServerCommon;

namespace nGrpc.Sessions
{
    internal class Session
    {
        public PlayerData PlayerData { get; private set; }
        public ITimer Timer { get; private set; }

        internal Session(PlayerData playerData, ITimer timer)
        {
            PlayerData = playerData;
            Timer = timer;
        }
    }
}
