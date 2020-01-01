using nGrpc.ServerCommon;
using System.Threading;

namespace nGrpc.Sessions
{
    internal class Session
    {
        private PlayerData _playerData;
        private readonly ITimer _timer;
        private readonly int _timeoutInMilisec;

        internal Session(PlayerData playerData, ITimer timer, int timeoutInMilisec)
        {
            _playerData = playerData;
            _timer = timer;
            _timeoutInMilisec = timeoutInMilisec;

            ResetTimer();
        }

        internal PlayerData GetPlayerData()
        {
            return _playerData;
        }

        internal void ResetTimer()
        {
            _timer.Change(_timeoutInMilisec, Timeout.Infinite);
        }
    }
}
