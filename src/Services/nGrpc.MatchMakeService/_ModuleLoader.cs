using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using nGrpc.Common;
using nGrpc.ServerCommon;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nGrpc.MatchMakeService
{
    public class ModuleLoader : IModuleLoader
    {
        public void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddConfig<MatchMakeConfigs>(configuration, "MatchMakeConfigs");
            services.AddSingleton<IRoomCreator, RoomCreator>();
            services.AddSingleton<IMatchMaker, MatchMaker>();
            services.AddSingleton<IGrpcService, MatchMakeGrpcService>();
        }

        public List<(int priorityIndex, Func<Task> func)> Initializer(IServiceProvider serviceProvider)
        {
            return null;
        }
    }
}
