using Grpc.Core;
using nGrpc.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nGrpc.ServerCommon
{
    public interface IServerEventStreamsManager
    {
        int StreamsCount { get; }

        Task SendEventToAllPlayers(ServerEventRes serverEvent);
        Task SendEventToPlayer(int playerId, ServerEventRes serverEvent);
        Task SendEventToPlayers(IEnumerable<int> playerIds, ServerEventRes serverEvent);
        void AddStream(int playerId, IServerStreamWriter<ServerEventRes> streamWriter);
        void RemoveStream(int playerId);
    }
}
