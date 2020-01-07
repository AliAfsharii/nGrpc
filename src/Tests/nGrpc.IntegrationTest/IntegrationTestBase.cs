using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using nGrpc.Worker;

namespace nGrpc.IntegrationTest
{
    public class IntegrationTestBase
    {
        static readonly object _lock = new object();
        static bool IsServerRunning = false;

        public IServiceProvider ServiceProvider
        {
            get
            {
                return Program.ServiceProviderForTests;
            }
        }

        KestrelConfigs KestrelConfigs
        {
            get
            {
                return ServiceProvider.GetRequiredService<KestrelConfigs>();
            }
        }

        public IntegrationTestBase()
        {
            lock (_lock)
                if (IsServerRunning == false)
                {
                    string args = IntegrationTestServerExtender.CommandLineArgs;
                    Task.Run(() => Program.Main(args.Split(' ')));
                    Task timeoutTask = Task.Delay(10000);

                    while (timeoutTask.IsCompleted == false)
                    {
                        try
                        {
                            using (TcpClient tcpClient = new TcpClient())
                            {
                                tcpClient.Connect(KestrelConfigs.Host, KestrelConfigs.Port);
                                Console.WriteLine("Port open");
                                Task.Delay(500).Wait();
                                break;
                            }
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Port closed");
                            Task.Delay(1000).Wait();
                        }
                    }

                    if (timeoutTask.Exception != null)
                        throw new Exception("Integration Test Server Timeout Reached.");

                    IsServerRunning = true;
                }
        }


        public TService GetServiceFromProvider<TService>()
        {
            return Program.ServiceProviderForTests.GetRequiredService<TService>();
        }
    }
}
