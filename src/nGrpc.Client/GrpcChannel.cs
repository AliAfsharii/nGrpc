using Grpc.Core;
using Grpc.Core.Interceptors;
using System;
using System.Threading.Tasks;
using nGrpc.Common;
using nGrpc.Client.Interceptors;

namespace nGrpc.Client
{
    public class GrpcChannel
    {
        public string Host { get; private set; }
        public int Port { get; private set; }

        Channel Channel { get; set; }
        public CallInvoker Invoker { get; private set; }
        public PlayerCredentials PlayerCredential { get; set; }


        public GrpcChannel()
        {
        }

        public async Task Connect(string host, int port, bool enableTLS = false)
        {
            Host = host;
            Port = port;

            Channel = new Channel(Host, Port, ChannelCredentials.Insecure);
            Invoker = Channel.Intercept(new AuthInterceptor(this).AddHeader).Intercept(new LoggerInterceptor());
            await Channel.ConnectAsync(DateTime.UtcNow + TimeSpan.FromSeconds(5));
            ClientLogger.Logger.LogInfo(Channel.State.ToString());
        }

        public async Task Disconnect()
        {
            ClientLogger.Logger.LogInfo(Channel.State.ToString());
            await Task.Delay(1000);
            await Channel.ShutdownAsync();
            ClientLogger.Logger.LogInfo(Channel.State.ToString());
        }
    }
}
