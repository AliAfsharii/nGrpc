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
        public PlayerCredentials PlayerCredential { get; set; }


        Channel _channel { get; set; }
        CallInvoker _invoker { get; set; }
        string _callHost;
        CallOptions _callOption;


        public GrpcChannel()
        {
        }

        public async Task Connect(string host, int port, bool enableTLS = false)
        {
            Host = host;
            Port = port;

            _channel = new Channel(Host, Port, ChannelCredentials.Insecure);
            _invoker = _channel.Intercept(new AuthInterceptor(this).AddHeader).Intercept(new LoggerInterceptor());
            await _channel.ConnectAsync(DateTime.UtcNow + TimeSpan.FromSeconds(5));
            ClientLogger.Logger.LogInfo(_channel.State.ToString());
        }

        public async Task Disconnect()
        {
            ClientLogger.Logger.LogInfo(_channel.State.ToString());
            await Task.Delay(1000);
            await _channel.ShutdownAsync();
            ClientLogger.Logger.LogInfo(_channel.State.ToString());
        }

        public async Task<TRes> CallRpc<TReq, TRes>(Method<TReq, TRes> descriptor, TReq request)
        where TReq : class
        where TRes : class
        {
            TRes res = await _invoker.AsyncUnaryCall(descriptor, _callHost, _callOption, request);
            return res;
        }
    }
}
