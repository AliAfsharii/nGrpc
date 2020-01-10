using nGrpc.Common;
using System;
using System.Threading.Tasks;

namespace nGrpc.ServerCommon
{
    public interface IProfileRepository
    {
        Task<(int playerId, Guid secretKey)> Register();
        Task<PlayerData> Login(int playerId, Guid secretKey);
        Task SavePlayerData(PlayerData playerData);
    }
}
