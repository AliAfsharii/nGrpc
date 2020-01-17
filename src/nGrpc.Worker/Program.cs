using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using nGrpc.ServerCommon;
using Serilog;

namespace nGrpc.Worker
{
    public class Program
    {
        public static IServiceProvider ServiceProviderForTests;
        public static bool ServerIsReady;

        public static void Main(string[] args)
        {
            Console.WriteLine("Server Is Running...");

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            Dictionary<string, string> cmdArgsDic = PairCommandArgs(args);

            return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config
                .AddJsonFile($"{hostingContext.HostingEnvironment.ContentRootPath}/nGrpcServerConfigs.json", false);
                config.AddEnvironmentVariables();
            })
            .ConfigureServices(services =>
            {
                CommandLineArguments arguments = new CommandLineArguments
                {
                    DropDatabase = cmdArgsDic.ContainsKey("dropdatabase")
                };
                services.AddSingleton(arguments);
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>()
                .UseKestrel((context, options) =>
                {
                    var kc = options.ApplicationServices.GetRequiredService<KestrelConfigs>();
                    options.Limits.MaxConcurrentConnections = kc.MaxConcurrentConnections;
                    options.Listen(IPAddress.Parse(kc.Host), kc.Port);

                    Console.WriteLine($"Kestrel is listening on: http://{kc.Host}:{kc.Port}");
                });
            })
            .UseSerilog()
            ;
        }

        public static Dictionary<string, string> PairCommandArgs(string[] args)
        {
            Dictionary<string, string> cmdArgs = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                string currentArg = args[i];
                if (string.IsNullOrEmpty(currentArg) == false && currentArg[0] == '-')
                {
                    string nextArg = args.Length > i + 1 ? args[i + 1] : null;
                    if (string.IsNullOrEmpty(nextArg) == false)
                        cmdArgs.Add(args[i].Remove(0, 1).ToLower(), args[i + 1].ToLower());
                    else
                        cmdArgs.Add(args[i].Remove(0, 1).ToLower(), null);
                }
            }

            return cmdArgs;
        }
    }
}
