using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using nGrpc.ServerCommon;
using System;
using System.Threading.Tasks;

namespace nGrpc.ProfileService
{
    public class ModuleLoader : IModuleLoader
    {
        public void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IProfileRepository, ProfileRepository>();
            services.AddSingleton<IProfile, Profile>();
            services.AddSingleton<IGrpcService, ProfileGrpc>();
        }

        public System.Collections.Generic.List<(int priorityIndex, Func<Task> func)> Initializer(IServiceProvider serviceProvider)
        {
            return null;
        }
    }
}
