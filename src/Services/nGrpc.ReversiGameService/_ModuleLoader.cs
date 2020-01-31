using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using nGrpc.ServerCommon;
using System;
using System.Threading.Tasks;

namespace nGrpc.ReversiGameService
{
    public class ModuleLoader : IModuleLoader
    {
        public void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddConfig<ReversiGameConfigs>(configuration, "ReversiGameConfigs");
            services.AddSingleton<IReversiGamesManager, ReversiGamesManager>();
            services.AddSingleton<IMatchProvider>(sp => (ReversiGamesManager)sp.GetRequiredService<IReversiGamesManager>());
            services.AddSingleton<IReversiLogicCreator, ReversiLogicCreator>();
        }

        public System.Collections.Generic.List<(int priorityIndex, Func<Task> func)> Initializer(IServiceProvider serviceProvider)
        {
            return null;
        }
    }
}
