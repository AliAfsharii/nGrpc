using Grpc.Core;
using Microsoft.Extensions.Logging;
using nGrpc.Common;
using nGrpc.Common.Descriptors;
using nGrpc.ServerCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nGrpc.MatchMakeService
{
    public class MatchMakeGrpcService : IGrpcService
    {
        private readonly ILogger<MatchMakeGrpcService> _logger;
        private readonly IServerEventStreamsManager _serverEventStreamsManager;
        private readonly IMatchMaker _matchMaker;

        public MatchMakeGrpcService(
            ILogger<MatchMakeGrpcService> logger,
            IServerEventStreamsManager serverEventStreamsManager,
            IMatchMaker matchMaker)
        {
            _logger = logger;
            _serverEventStreamsManager = serverEventStreamsManager;
            _matchMaker = matchMaker;
        }


        // private

        private async Task SendMatchMakeUpdateEvent(List<MatchMakePlayer> matchMakePlayers, int? matchId)
        {
            try
            {
                var matchMakeUpdateEvent = new MatchMakeUpdateEvent
                {
                    MatchMakePlayers = matchMakePlayers,
                    MatchId = matchId
                };
                var serverEvent = new ServerEventRes
                {
                    ServerEventType = ServerEventType.MatchMake,
                    CustomData = matchMakeUpdateEvent.ToJson()
                };
                List<int> playerIds = matchMakePlayers.Select(n => n.Id).ToList();
                await _serverEventStreamsManager.SendEventToPlayers(playerIds, serverEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }



        // public

        public void AddRpcMethods(IGrpcBuilderAdapter grpcBuilder)
        {
            grpcBuilder.AddMethod(MatchMakeGrpcDescriptors.MatchMakeDesc, MatchMakeRPC);
            grpcBuilder.AddMethod(MatchMakeGrpcDescriptors.LeaveDesc, LeaveRPC);
        }


        public async Task<MatchMakeRes> MatchMakeRPC(MatchMakeReq req, ServerCallContext context)
        {
            int playerId = context.GetPlayerCredential().PlayerId;

            (List<MatchMakePlayer> matchMakePlayers, int? matchId) = await _matchMaker.MatchMake(playerId);
            _logger.LogInformation("MatchMakeRPC, PlayerId:{pid}", playerId);

            SendMatchMakeUpdateEvent(matchMakePlayers, matchId).NoAwait();

            return new MatchMakeRes
            {
            };
        }

        public async Task<LeaveRes> LeaveRPC(LeaveReq req, ServerCallContext context)
        {
            int playerId = context.GetPlayerCredential().PlayerId;

            List<MatchMakePlayer> matchMakePlayers = await _matchMaker.Leave(playerId);
            _logger.LogInformation("MatchMake LeaveRPC, PlayerId:{pid}", playerId);

            SendMatchMakeUpdateEvent(matchMakePlayers, null).NoAwait();

            return new LeaveRes
            {
            };
        }
    }
}
