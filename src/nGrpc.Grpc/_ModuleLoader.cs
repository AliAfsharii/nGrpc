using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using nGrpc.ServerCommon;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nGrpc.Grpc
{
    public class ModuleLoader : IModuleLoader
    {
        public void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddConfig<GrpcConfigs>(configuration, "GrpcConfigs");
            services.AddSingleton<GrpcServer>();
            services.AddSingleton<ServerInterceptor>();
            services.AddSingleton<IServerEventStreamsManager, ServerEventStreamsManager>();
            services.AddSingleton<IGrpcService, BaseGrpcService>();
        }

        public List<(int priorityIndex, Func<Task> func)> Initializer(IServiceProvider serviceProvider)
        {
            async Task func()
            {
                //// Run grpc server
                GrpcServer grpcServer = serviceProvider.GetRequiredService<GrpcServer>();
                grpcServer.Run();
            }

            return new List<(int priorityIndex, Func<Task> func)>
            {
                (100, func)
            };
        }
    }
}
