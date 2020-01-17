using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nGrpc.ServerCommon
{
    internal class ModuleLoader : IModuleLoader
    {
        public void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ITimerProvider, TimerProvider>();
            services.AddSingleton<ITime, Time>();
            services.AddSingleton<IPubSubHub, PubSubHub>();
        }

        public List<(int priorityIndex, Func<Task> func)> Initializer(IServiceProvider serviceProvider)
        {
            return null;
        }
    }
}
