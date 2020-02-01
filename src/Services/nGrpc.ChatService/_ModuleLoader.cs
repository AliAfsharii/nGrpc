using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using nGrpc.ServerCommon;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nGrpc.ChatService
{
    public class ModuleLoader : IModuleLoader
    {
        public void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IChatRepository, ChatRepository>();
            services.AddSingleton<IChatHub, ChatHub>();
            services.AddSingleton<IGrpcService, ChatGrpcService>();
            services.AddConfig<ChatConfigs>(configuration, "ChatConfigs");
            services.AddSingleton<IChatRoomFactory, ChatRoomFactory>();
        }

        public List<(int priorityIndex, Func<Task> func)> Initializer(IServiceProvider serviceProvider)
        {
            return null;
        }
    }
}
