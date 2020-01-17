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
            services.AddSingleton<ChatHub>();
            services.AddSingleton<IGrpcService, ChatGrpcService>();
            services.AddConfig<ChatConfigs>(configuration, "ChatConfigs");
            services.AddSingleton<Func<string, ChatRoom>>(sp =>
            {
                Func<string, ChatRoom> func = roomName =>
                {
                    ILogger<ChatRoom> logger = sp.GetRequiredService<ILogger<ChatRoom>>();
                    ITime time = sp.GetRequiredService<ITime>();
                    ChatConfigs cc = sp.GetRequiredService<ChatConfigs>();
                    IChatRepository chatRepository = sp.GetRequiredService<IChatRepository>();
                    IPubSubHub pubsub = sp.GetRequiredService<IPubSubHub>();

                    ChatRoom chatRoom = new ChatRoom(logger, roomName, time, cc, chatRepository, pubsub);
                    return chatRoom;
                };
                return func;
            });
        }

        public List<(int priorityIndex, Func<Task> func)> Initializer(IServiceProvider serviceProvider)
        {
            return null;
        }
    }
}
