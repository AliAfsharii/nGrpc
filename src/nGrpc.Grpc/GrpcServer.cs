using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using nGrpc.ServerCommon;

namespace nGrpc.Grpc
{
    public class GrpcServer //: IGrpcServer
    {
        readonly ILogger<GrpcServer> _logger;
        readonly GrpcConfigs _grpcConfigs;
        readonly IEnumerable<IGrpcService> _grpcServices;

        Server server;

        public GrpcServer(ILogger<GrpcServer> logger,
            GrpcConfigs grpcConfigs,
            IEnumerable<IGrpcService> grpcServices)
        {
            _logger = logger;
            _grpcConfigs = grpcConfigs;
            _grpcServices = grpcServices;
        }

        public void Run()
        {
            server = new Server()
            {
                Services =
                {
                    ServerServiceDefinition.CreateBuilder()
                        .AddServices(_grpcServices)
                        .Build()
                },
                Ports = { new ServerPort(_grpcConfigs.Host, _grpcConfigs.Port, SslServerCredentials.Insecure) }

            };

            server.RequestCallTokensPerCompletionQueue = 50000;
            server.Start();
            _logger.LogInformation("Grpc server started on {host}:{port}", _grpcConfigs.Host, _grpcConfigs.Port);
        }
    }
}
