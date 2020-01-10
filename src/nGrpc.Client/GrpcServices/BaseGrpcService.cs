using Grpc.Core;
using nGrpc.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace nGrpc.Client.GrpcServices
{
    public class BaseGrpcService
    {
        public event Action<string> OnTestServerEventReceived;

        private readonly GrpcChannel _grpcChannel;

        public BaseGrpcService(GrpcChannel channel)
        {
            _grpcChannel = channel;

            _grpcChannel.OnServerEventReceived += ServerEventReceivedCallback;
        }

        private void ServerEventReceivedCallback(ServerEventType serverEventType, string eventData)
        {
            if (serverEventType == ServerEventType.Test)
            {
                OnTestServerEventReceived?.Invoke(eventData);
            }
        }

        bool isStreamConnected;
        CancellationTokenSource serverEventCts;
        CancellationToken serverEventCt;
        AsyncDuplexStreamingCall<ServerEventReq, ServerEventRes> serverEventStream;
        internal async Task ConnectServerEventStreamRPC()
        {
            if (serverEventStream != null)
                throw new Exception("ServerEvent already is connected");

            serverEventStream = _grpcChannel.CallStreamRpc(BaseGrpcDescriptors.ServerEventStreamDesc);

            serverEventCts = new CancellationTokenSource();
            serverEventCt = serverEventCts.Token;

            try
            {
                await serverEventStream.ResponseStream.MoveNext(serverEventCt);
            }
            catch (Exception ex)
            {
                ClientLogger.Logger.LogError(ex, ex.StackTrace);
            }

            isStreamConnected = true;
            KeepServerEventStreamAlive();
            
            WaitForReceiveEvent();
        }

        async void WaitForReceiveEvent()
        {
            try
            {
                while (await serverEventStream.ResponseStream.MoveNext(serverEventCt))
                {
                    ServerEventRes serverEventRes = serverEventStream.ResponseStream.Current;
                    _grpcChannel.CustomServerEventReceived(serverEventRes.ServerEventType, serverEventRes.CustomData);
                }
            }
            catch (Exception ex)
            {
                ClientLogger.Logger.LogError(ex, ex.StackTrace);
            }
            finally
            {
                serverEventStream.Dispose();
                serverEventStream = null;
            }
        }

        CancellationTokenSource keepAliveCts;
        CancellationToken keepAliveCt;
        async void KeepServerEventStreamAlive()
        {
            keepAliveCts = new CancellationTokenSource();
            keepAliveCt = keepAliveCts.Token;
            while (keepAliveCt.IsCancellationRequested == false)
            {
                try
                {
                    await Task.Delay(500);
                    await serverEventStream.RequestStream.WriteAsync(new ServerEventReq());
                    await Task.Delay(30 * 1000, serverEventCt);
                }
                catch (Exception ex)
                {
                    ClientLogger.Logger.LogError(ex, ex.StackTrace);
                }
            }
        }

        internal async Task DisconnectServerEventStreamRPC()
        {
            keepAliveCts?.Cancel();

            if (serverEventStream != null)
            {
                await serverEventStream.RequestStream.CompleteAsync();
            }
            isStreamConnected = false;
        }


        public async Task<ServerEventTestRes> ServerEventTestRPC(ServerEventTestReq req)
        {
            var res = await _grpcChannel.CallRpc(BaseGrpcDescriptors.ServerEventTestDesc, req);
            return res;
        }
    }
}
