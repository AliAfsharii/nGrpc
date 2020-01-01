using nGrpc.ServerCommon;
using System;
using System.Collections.Concurrent;

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

            ITimer timer = _timerProvider.GetNewTimer();
            timer.SetCallback(() => RemoveSession(playerId));

            Session newSession = new Session(playerData, timer, _sessionConfigs.TimeoutInMilisec);

            _sessionsDic.AddOrUpdate(playerId, newSession, (id, session) => newSession);
        }

        public PlayerData GetPlayerData(int playerId)
        {
            Session session = GetSession(playerId);
            if (session == null)
                throw new ThereIsNoPlayerDataForSuchPlayerException($"PlayerId: {playerId}");
            return session.GetPlayerData().CloneByMessagePack();
        }

        public void ResetTimer(int playerId)
        {
            Session session = GetSession(playerId);
            if (session == null)
                throw new ThereIsNoPlayerDataForSuchPlayerException($"PlayerId: {playerId}");
            session.ResetTimer();
        }
    }
}
