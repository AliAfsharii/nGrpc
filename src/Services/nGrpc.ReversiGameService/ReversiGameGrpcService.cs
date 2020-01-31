using Grpc.Core;
using Microsoft.Extensions.Logging;
using nGrpc.Common;
using nGrpc.Common.Descriptors;
using nGrpc.ServerCommon;
using System.Threading.Tasks;

namespace nGrpc.ReversiGameService
{
    public class ReversiGameGrpcService : IGrpcService
    {
        private readonly ILogger<ReversiGameGrpcService> _logger;
        private readonly IReversiGamesManager _reversiGamesManager;

        public ReversiGameGrpcService(ILogger<ReversiGameGrpcService> logger,
            IReversiGamesManager reversiGamesManager)
        {
            _logger = logger;
            _reversiGamesManager = reversiGamesManager;
        }


        public void AddRpcMethods(IGrpcBuilderAdapter grpcBuilder)
        {
            grpcBuilder.AddMethod(ReversiGameGrpcDescriptors.GetGameDataDesc, GetGameDataRPC);
            grpcBuilder.AddMethod(ReversiGameGrpcDescriptors.PutDiskDesc, PutDiskRPC);
        }


        public async Task<GetGameDataRes> GetGameDataRPC(GetGameDataReq req, ServerCallContext context)
        {
            int playerId = context.GetPlayerCredential().PlayerId;
            int gameId = req.GameId;

            ReversiGameData reversiGameData = _reversiGamesManager.GetGameData(playerId, gameId);
            _logger.LogInformation("GetGameDataRPC, PlayerId:{pid}, GameId:{gi}", playerId, gameId);

            return new GetGameDataRes
            {
                GameData = reversiGameData
            };
        }

        public async Task<PutDiskRes> PutDiskRPC(PutDiskReq req, ServerCallContext context)
        {
            int playerId = context.GetPlayerCredential().PlayerId;
            int gameId = req.GameId;
            int row = req.Row;
            int col = req.Col;

            ReversiGameData reversiGameData = _reversiGamesManager.PutDisk(playerId, gameId, row, col);
            _logger.LogInformation("PutDiskRPC, PlayerId:{pid}, Req:{req}", playerId, req.ToJson());

            return new PutDiskRes
            {
                GameData = reversiGameData
            };
        }
    }
}
