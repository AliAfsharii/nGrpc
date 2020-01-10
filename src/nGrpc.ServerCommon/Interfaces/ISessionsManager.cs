using nGrpc.Common;
using System;
using System.Threading.Tasks;

namespace nGrpc.ServerCommon
{
    public interface ISessionsManager
    {
        Guid AddSession(PlayerData playerData);
        PlayerData GetPlayerData(int playerId);
        void ResetTimer(int playerId);
        bool HasSessionBySessionId(int playerId, Guid sessionId);
        bool HasSessionBySecretKey(int playerId, Guid secretKey, out Guid sessionId);
        Task<PlayerData> ManipulatePlayerData(int playerId, Action<PlayerData> action);
    }
}