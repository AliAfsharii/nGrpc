using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using nGrpc.Worker;

namespace nGrpc.IntegrationTest
{
    public class IntegrationTestBase
    {
        static readonly object _lock = new object();
        static bool _isServerRunning = false;
        static Exception _exception;

        public IServiceProvider ServiceProvider
        {
            get
            {
                return Program.ServiceProviderForTests;
            }
        }

        public IntegrationTestBase()
        {
            lock (_lock)
                if (_isServerRunning == false)
                {
                    if (_exception != null)
                        throw _exception;

                    string args = IntegrationTestServerExtender.CommandLineArgs;
                    Task serverRunTask = Task.Run(() => Program.Main(args.Split(' ')));
                    Task timeoutTask = Task.Delay(15000);

                    while (Program.ServerIsReady == false && timeoutTask.IsCompleted == false && serverRunTask.Exception == null)
                        Task.Delay(1000).Wait();

                    if (serverRunTask.Exception != null)
                    {
                        _exception = new AggregateException("Server encountered an exception.", serverRunTask.Exception);
                        throw _exception;
                    }
                    else if (timeoutTask.IsCompleted == true)
                    {
                        _exception = new Exception("Integration Test Server Timeout Reached.");
                        throw _exception;
                    }

                    _isServerRunning = true;
                }
        }


        public TService GetServiceFromProvider<TService>()
        {
            return Program.ServiceProviderForTests.GetRequiredService<TService>();
        }
    }
}
