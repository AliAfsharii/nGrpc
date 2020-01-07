using System;
using System.Threading.Tasks;

namespace nGrpc.ProfileService
{
    public interface IProfile
    {
        Task<Guid> Login(int playerId, Guid secretKey);
        Task<(int playerId, Guid secretKey)> Register();
    }
}