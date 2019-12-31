using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using nGrpc.ServerCommon;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nGrpc.DB
{
    public class ModuleLoader : IModuleLoader
    {
        public void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddConfig<DBConfigs>(configuration, "DBConfigs");
            services.AddSingleton<IDBMigrator, DBMigrator>();
        }

        public System.Collections.Generic.List<(int priorityIndex, Func<Task> func)> Initializer(IServiceProvider serviceProvider)
        {
            async Task func()
            {
                IDBMigrator dbMigrator = serviceProvider.GetRequiredService<IDBMigrator>();
                var commandArguments = serviceProvider.GetRequiredService<CommandLineArguments>();
                ModulesData modulesData = serviceProvider.GetRequiredService<ModulesData>();
                dbMigrator.MigrateDB(commandArguments.DropDatabase, modulesData.AllModulesNames);
            }

            var list = new List<(int priorityIndex, Func<Task> func)>
            {
                (-100, func)
            };
            return list;
        }
    }
}
