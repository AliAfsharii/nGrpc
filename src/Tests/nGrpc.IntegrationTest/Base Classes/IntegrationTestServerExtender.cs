using Microsoft.Extensions.DependencyInjection;
using nGrpc.ChatService;
using System;

namespace nGrpc.IntegrationTest
{
    public static class IntegrationTestServerExtender
    {
        public static string CommandLineArgs = "-DropDatabase";

        public static void ServerInitializer(IServiceProvider serviceProvider)
        {
            serviceProvider.GetRequiredService<ChatConfigs>().ChatSaveIntervalInMilisec = 10;
        }
    }
}
