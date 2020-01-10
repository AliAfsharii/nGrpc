using Grpc.Core;
using Microsoft.Extensions.Logging;
using nGrpc.Common;
using nGrpc.ServerCommon;
using System.Threading;
using System.Threading.Tasks;

namespace nGrpc.Grpc
{
    public class BaseGrpcService : IGrpcService
    {
        readonly ILogger<BaseGrpcService> _logger;
        readonly IServerEventStreamsManager _serverEventStreamsManager;
        readonly ISessionsManager _sessionsManager;
        private readonly GrpcConfigs _grpcConfigs;

        public BaseGrpcService(ILogger<BaseGrpcService> logger,
            IServerEventStreamsManager serverEventStreamsManager,
            ISessionsManager sessionsManager,
            GrpcConfigs grpcConfigs)
        {
            _logger = logger;
            _serverEventStreamsManager = serverEventStreamsManager;
            _sessionsManager = sessionsManager;
            _grpcConfigs = grpcConfigs;
        }

        void IGrpcService.AddRpcMethods(IGrpcBuilderAdapter grpcBuilder)
        {
            grpcBuilder.AddMethod(BaseGrpcDescriptors.ServerEventStreamDesc, ServerEventStreamRPC);
            grpcBuilder.AddMethod(BaseGrpcDescriptors.ServerEventTestDesc, ServerEventTestRPC);
        }

        public async Task ServerEventStreamRPC(IAsyncStreamReader<ServerEventReq> reqStream, IServerStreamWriter<ServerEventRes> resStream, ServerCallContext context)
        {
            await resStream.WriteAsync(new ServerEventRes());
            int playerId = context.GetPlayerCredential().PlayerId;
            _serverEventStreamsManager.AddStream(playerId, resStream);
            _logger.LogInformation("Player {pid} connect ServerEventStream", playerId);

            CancellationTokenSource cts = new CancellationTokenSource();
            int timeout = _grpcConfigs.ServerEventStreamTimeout;
            Timer timer = new Timer(n => cts.Cancel(), null, timeout, Timeout.Infinite);

            // wait until client complete stream
            while (await reqStream.MoveNext(cts.Token) == true)
            {
                timer.Change(timeout, Timeout.Infinite);
                _sessionsManager.ResetTimer(playerId);
                // _logger.LogInformation("Player {pid} Stream request received.", playerId);
            }

            _serverEventStreamsManager.RemoveStream(playerId);
            _logger.LogInformation("Player {pid} disconnect ServerEventStream", playerId);
        }

        public async Task<ServerEventTestRes> ServerEventTestRPC(ServerEventTestReq req, ServerCallContext context)
        {
            int playerId = context.GetPlayerCredential().PlayerId;
            string customData = req.CustomData;

            ServerEventRes serverEvent = new ServerEventRes
            {
                ServerEventType = ServerEventType.Test,
                CustomData = customData
            };
            await _serverEventStreamsManager.SendEventToPlayer(playerId, serverEvent);
            return new ServerEventTestRes();
        }
    }
}
