using System.Collections.Generic;
using System.Threading.Tasks;

namespace nGrpc.ServerCommon
{
    public interface IMatchProvider
    {
        Task<int> CreateMatch(List<int> playerIds);
    }
}
