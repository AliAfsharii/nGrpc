using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using nGrpc.ServerCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace nGrpc.Worker.StartupUtils
{
    public static class ModuleLoaderUtils
    {
        public static List<string> GetAllModuleNamesFromConfigs(IConfiguration configuration)
        {
            List<string> allModulesNames = new List<string>();
            allModulesNames.AddRange(configuration.GetSection("ModuleNames").GetChildren().Select(n => n.Value).ToList());

            string duplicateAssemblyName = allModulesNames.GroupBy(n => n).Where(n => n.Count() > 1).Select(n => n.Key).FirstOrDefault();
            if (duplicateAssemblyName != null)
                throw new Exception($"There are duplicate values in assemblies name in config file. AssemblyName:{duplicateAssemblyName}");

            return allModulesNames;
        }

        public static List<IModuleLoader> Execute_AllModuleLoaders_AddServices(List<string> allModuleNames, IServiceCollection services, IConfiguration configuration)
        {
            List<Assembly> assemblies = new List<Assembly>();
            foreach (string moduleName in allModuleNames)
            {
                Assembly assembly = Assembly.Load(moduleName);
                assemblies.Add(assembly);
            }

            Type iModuleLoaderType = typeof(IModuleLoader);
            IEnumerable<Type> moduleLoaderTypes = assemblies
                .SelectMany(s => s.GetTypes())
                .Where(p => iModuleLoaderType.IsAssignableFrom(p) && !p.IsInterface);

            var allModuleLoaders = new List<IModuleLoader>();
            foreach (Type moduleLoaderType in moduleLoaderTypes)
            {
                IModuleLoader moduleLoader = (IModuleLoader)Activator.CreateInstance(moduleLoaderType);
                allModuleLoaders.Add(moduleLoader);
                moduleLoader.AddServices(services, configuration);
            }

            return allModuleLoaders;
        }

        public static async Task Execute_AllModuleLoaders_Initializer(IServiceProvider serviceProvider)
        {
            var modulesData = serviceProvider.GetRequiredService<ModulesData>();

            List<(int priorityIndex, Func<Task> func)> allTasks = new List<(int priorityIndex, Func<Task>)>();
            foreach (IModuleLoader moduleLoader in modulesData.AllModuleLoaders)
            {
                List<(int priorityIndex, Func<Task> task)> tasks = moduleLoader.Initializer(serviceProvider);
                if (tasks != null)
                    allTasks.AddRange(tasks);
            }
            List<Func<Task>> orderedTasks = allTasks.Where(n => n.func != null).OrderBy(n => n.priorityIndex).Select(n => n.func).ToList();
            foreach (var t in orderedTasks)
                await t();
        }
    }
}
