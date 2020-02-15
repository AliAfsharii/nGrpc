using Grpc.Core;
using Grpc.Core.Interceptors;
using System;
using System.Threading.Tasks;
using nGrpc.Common;
using nGrpc.Client.Interceptors;
using nGrpc.Client.GrpcServices;

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

        BaseGrpcService _baseGrpcService;

        public GrpcChannel()
        {
        }

        public async Task Connect(string host, int port)
        {
            Host = host;
            Port = port;

            _channel = new Channel(Host, Port, ChannelCredentials.Insecure);
            _invoker = _channel.Intercept(new AuthInterceptor(this).AddHeader).Intercept(new LoggerInterceptor());
            await _channel.ConnectAsync(DateTime.UtcNow + TimeSpan.FromSeconds(5));
            ClientLogger.Logger.LogInfo(_channel.State.ToString());

            _baseGrpcService = new BaseGrpcService(this);
        }

        public async Task Disconnect()
        {
            ClientLogger.Logger.LogInfo(_channel.State.ToString());
            await _baseGrpcService.DisconnectServerEventStreamRPC();
            await Task.Delay(1000);
            await _channel.ShutdownAsync();
            ClientLogger.Logger.LogInfo(_channel.State.ToString());
        }

        /// <summary>
        /// First Parameter: Event Type, Second Parameter: JsonData
        /// </summary>
        public event Action<ServerEventType, string> OnServerEventReceived;
        internal void CustomServerEventReceived(ServerEventType serverEventType, string jsonData)
        {
            ClientLogger.Logger.LogInfo($"[GRPC::EVENT] {serverEventType}: {jsonData}");
            OnServerEventReceived?.Invoke(serverEventType, jsonData);
        }

        public async Task ConnectServerEventStream()
        {
            await _baseGrpcService.ConnectServerEventStreamRPC();
        }

        public async Task<TRes> CallRpc<TReq, TRes>(Method<TReq, TRes> descriptor, TReq request)
        where TReq : class
        where TRes : class
        {
            TRes res = await _invoker.AsyncUnaryCall(descriptor, _callHost, _callOption, request);
            return res;
        }

        public AsyncDuplexStreamingCall<TReq, TRes> CallStreamRpc<TReq, TRes>(Method<TReq, TRes> descriptor)
        where TReq : class
        where TRes : class
        {
            var res = _invoker.AsyncDuplexStreamingCall(descriptor, _callHost, _callOption);
            return res;
        }
    }
}
