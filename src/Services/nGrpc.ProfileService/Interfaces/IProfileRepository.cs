using nGrpc.Common;
using System;
using System.Threading.Tasks;

namespace nGrpc.ProfileService
{
    public interface IProfileRepository
    {
        Task<(int playerId, Guid secretKey)> Register();
        Task<PlayerData> Login(int playerId, Guid secretKey);
    }
}
