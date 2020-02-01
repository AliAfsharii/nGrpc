using nGrpc.Common;
using System;
using System.Threading.Tasks;

namespace nGrpc.ProfileService
{
    public interface IProfile
    {
        Task<Guid> Login(int playerId, Guid secretKey);
        Task<(int playerId, Guid secretKey)> Register();
        Task<PlayerData> ChangeName(int playerId, string newName);
    }
}