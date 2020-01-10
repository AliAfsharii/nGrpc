using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using nGrpc.ServerCommon;
using Grpc.Core.Interceptors;

namespace nGrpc.Grpc
{
    public class GrpcServer
    {
        readonly ILogger<GrpcServer> _logger;
        readonly GrpcConfigs _grpcConfigs;
        readonly IEnumerable<IGrpcService> _grpcServices;
        private readonly ServerInterceptor _serverInterceptor;
        Server _server;

        public GrpcServer(ILogger<GrpcServer> logger,
            GrpcConfigs grpcConfigs,
            IEnumerable<IGrpcService> grpcServices,
            ServerInterceptor serverInterceptor)
        {
            _logger = logger;
            _grpcConfigs = grpcConfigs;
            _grpcServices = grpcServices;
            _serverInterceptor = serverInterceptor;
        }

        public void Run()
        {
            _server = new Server()
            {
                Services =
                {
                    ServerServiceDefinition.CreateBuilder()
                        .AddServices(_grpcServices)
                        .Build()
                        .Intercept(_serverInterceptor)
                },
                Ports = { new ServerPort(_grpcConfigs.Host, _grpcConfigs.Port, SslServerCredentials.Insecure) }

            };

            _server.RequestCallTokensPerCompletionQueue = 50000;
            _server.Start();
            _logger.LogInformation("Grpc server started on {host}:{port}", _grpcConfigs.Host, _grpcConfigs.Port);
        }
    }
}
