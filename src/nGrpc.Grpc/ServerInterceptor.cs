using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using nGrpc.ServerCommon;
using nGrpc.Common.Descriptors;

namespace nGrpc.Grpc
{
    public class ServerInterceptor : Interceptor
    {
        readonly ILogger<ServerInterceptor> _logger;
        readonly ISessionsManager _sessionManager;

        public ServerInterceptor(
            ILogger<ServerInterceptor> logger,
            ISessionsManager sessionManager)
        {
            _logger = logger;
            _sessionManager = sessionManager;
        }

        void CheckLogin(ServerCallContext context)
        {
            string serviceName = context.Method.Split('/')[1];
            string methodName = context.Method;

            if (methodName == ProfileDescriptors.RegisterDesc.FullName || methodName == ProfileDescriptors.LoginDesc.FullName)
                return;

            var credential = context.GetPlayerCredential();

            if (credential == null)
                throw new InvalidCredentialGrpcException();

            if (_sessionManager.HasSessionBySessionId(credential.PlayerId, credential.SessionId) == false)
                throw new NotLoggedInGrpcException();

            _sessionManager.ResetTimer(credential.PlayerId);
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                CheckLogin(context);

                var res = await continuation(request, context);
                return res;
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "RPC Error. UnaryServerHanler Error. Method : {method}", context.Method);
                throw ex;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UnaryServerHanler Error. Method : {method}", context.Method);
                throw new RpcException(new Status(StatusCode.Unknown, $"Internal Server Error: {ex.ToString()} %%%% {ex.StackTrace}"), ex.Message);
            }
        }

        public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                CheckLogin(context);

                await continuation(requestStream, responseStream, context); ;
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "RPC Error. DuplexStreamingServerHanler Error. Method : {method}", context.Method);
                throw ex;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DuplexStreamingServerHanler Error. Method : {method}", context.Method);
                throw new RpcException(new Status(StatusCode.Unknown, "Internal Server Error"), ex.Message);
            }
        }


        public override Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context, ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            return null;
        }
        public override Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            return null;
        }
    }
}
