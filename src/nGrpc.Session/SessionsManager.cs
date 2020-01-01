using nGrpc.ServerCommon;
using System.Collections.Concurrent;

namespace nGrpc.Sessions
{
    public class SessionsManager
    {
        private readonly ConcurrentDictionary<int, Session> _sessionsDic = new ConcurrentDictionary<int, Session>();

        private Session GetSession(int playerId)
        {
            _sessionsDic.TryGetValue(playerId, out Session session);
            return session;
        }

        public void AddSession(PlayerData playerData)
        {
            Session newSession = new Session(playerData);
            _sessionsDic.AddOrUpdate(playerData.Id, newSession, (id, session) => newSession);
        }

        public PlayerData GetPlayerData(int playerId)
        {
            Session session = GetSession(playerId);
            return session.GetPlayerData().CloneByMessagePack();
        }
    }
}
