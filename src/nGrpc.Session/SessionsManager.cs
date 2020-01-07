using nGrpc.Common;
using nGrpc.ServerCommon;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace nGrpc.Sessions
{
    public class SessionsManager : ISessionsManager
    {
        private readonly ConcurrentDictionary<int, Session> _playerIdToSession = new ConcurrentDictionary<int, Session>();
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
            _playerIdToSession.TryGetValue(playerId, out Session session);
            return session;
        }

        private void RemoveSession(int playerId)
        {
            _playerIdToSession.TryRemove(playerId, out _);
        }


        // public
        public Guid AddSession(PlayerData playerData)
        {
            int playerId = playerData.Id;

            ITimer timer = _timerProvider.CreateTimer();
            timer.SetCallback(() => RemoveSession(playerId));
            timer.Change(_sessionConfigs.TimeoutInMilisec, Timeout.Infinite);

            Session newSession = new Session(playerData, timer);

            _playerIdToSession.AddOrUpdate(playerId, newSession, (id, session) => newSession);

            return newSession.Id;
        }

        public PlayerData GetPlayerData(int playerId)
        {
            Session session = GetSession(playerId);
            if (session == null)
                throw new ThereIsNoPlayerDataForSuchPlayerException($"PlayerId: {playerId}");

            using (session.Lock())
                return session.PlayerData.CloneByMessagePack();
        }

        public void ResetTimer(int playerId)
        {
            Session session = GetSession(playerId);
            if (session == null)
                throw new ThereIsNoPlayerDataForSuchPlayerException($"PlayerId: {playerId}");

            using (session.Lock())
                session.Timer.Change(_sessionConfigs.TimeoutInMilisec, Timeout.Infinite);
        }

        public bool HasSessionBySessionId(int playerId, Guid sessionId)
        {
            if (_playerIdToSession.TryGetValue(playerId, out Session session) == true)
                if (session.Id == sessionId)
                    return true;

            return false;
        }

        public bool HasSessionBySecretKey(int playerId, Guid secretKey, out Guid sessionId)
        {
            if (_playerIdToSession.TryGetValue(playerId, out Session session) == true)
                if (session.PlayerData.SecretKey == secretKey)
                {
                    sessionId = session.Id;
                    return true;
                }

            return false;
        }
    }
}
