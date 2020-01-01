using nGrpc.ServerCommon;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace nGrpc.Sessions
{
    public class SessionsManager
    {
        private readonly ConcurrentDictionary<int, Session> _sessionsDic = new ConcurrentDictionary<int, Session>();
        private readonly ITimerProvider _timerProvider;
        private readonly SessionConfigs _sessionConfigs;

        public SessionsManager(ITimerProvider timerProvider, SessionConfigs sessionConfigs)
        {
            _timerProvider = timerProvider;
            _sessionConfigs = sessionConfigs;
        }


        // private
        private Session GetSession(int playerId)
        {
            _sessionsDic.TryGetValue(playerId, out Session session);
            return session;
        }

        private void RemoveSession(int playerId)
        {
            _sessionsDic.TryRemove(playerId, out _);
        }


        // public
        public void AddSession(PlayerData playerData)
        {
            int playerId = playerData.Id;

            ITimer timer = _timerProvider.CreateTimer();
            timer.SetCallback(() => RemoveSession(playerId));
            timer.Change(_sessionConfigs.TimeoutInMilisec, Timeout.Infinite);

            Session newSession = new Session(playerData, timer);

            _sessionsDic.AddOrUpdate(playerId, newSession, (id, session) => newSession);
        }

        public PlayerData GetPlayerData(int playerId)
        {
            Session session = GetSession(playerId);
            if (session == null)
                throw new ThereIsNoPlayerDataForSuchPlayerException($"PlayerId: {playerId}");
            return session.PlayerData.CloneByMessagePack();
        }

        public void ResetTimer(int playerId)
        {
            Session session = GetSession(playerId);
            if (session == null)
                throw new ThereIsNoPlayerDataForSuchPlayerException($"PlayerId: {playerId}");

             session.Timer.Change(_sessionConfigs.TimeoutInMilisec, Timeout.Infinite);
        }
    }
}
