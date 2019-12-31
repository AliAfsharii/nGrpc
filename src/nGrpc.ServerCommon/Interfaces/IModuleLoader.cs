using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nGrpc.ServerCommon
{
    public interface IModuleLoader
    {
        void AddServices(IServiceCollection services, IConfiguration configuration);
        List<(int priorityIndex, Func<Task> func)> Initializer(IServiceProvider serviceProvider);
    }
}
