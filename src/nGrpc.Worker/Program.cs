using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace nGrpc.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Server Is Running...");

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config
                .AddJsonFile($"{hostingContext.HostingEnvironment.ContentRootPath}/nGrpcServerConfigs.json", false);
                config.AddEnvironmentVariables();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>()
                .UseKestrel((context, options) =>
                {
                    options.Limits.MaxConcurrentConnections = 100;

                    var kc = new KestrelConfigs();
                    context.Configuration.Bind("KestrelConfigs", kc);
                    options.Listen(IPAddress.Parse(kc.Host), kc.Port);

                    Console.WriteLine($"Kestrel is listening on: http://{kc.Host}:{kc.Port}");
                });
            });
        }
    }

    public class KestrelConfigs
    {
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
