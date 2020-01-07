using nGrpc.Common;
using System;

namespace nGrpc.ServerCommon
{
    public interface ISessionsManager
    {
        Guid AddSession(PlayerData playerData);
        PlayerData GetPlayerData(int playerId);
        void ResetTimer(int playerId);
        bool HasSessionBySessionId(int playerId, Guid sessionId);
        bool HasSessionBySecretKey(int playerId, Guid secretKey, out Guid sessionId);
    }
}