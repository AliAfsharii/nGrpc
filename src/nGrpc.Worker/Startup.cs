using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using nGrpc.Worker.StartupUtils;
using nGrpc.ServerCommon;

namespace nGrpc.Worker
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            var allModulesNames = ModuleLoaderUtils.GetAllModuleNamesFromConfigs(Configuration);
            var allModuleLoaders = ModuleLoaderUtils.Execute_AllModuleLoaders_AddServices(allModulesNames, services, Configuration);
            var allDiServices = services.Select(n => n.ServiceType).ToList();

            ModulesData modulesData = new ModulesData
            {
                AllModulesNames = allModulesNames,
                AllModuleLoaders = allModuleLoaders,
                AllDIServices = allDiServices
            };
            services.AddSingleton<ModulesData>(modulesData);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            IServiceProvider serviceProvider = app.ApplicationServices;

            var logLevelConfig = serviceProvider.GetRequiredService<LogLevelConfigs>();
            SerilogUtils.AddSerilog("", logLevelConfig);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            ModuleLoaderUtils.Execute_AllModuleLoaders_Initializer(serviceProvider).Wait();
        }
    }
}
