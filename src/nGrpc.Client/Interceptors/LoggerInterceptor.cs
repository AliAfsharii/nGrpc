using Grpc.Core;
using Grpc.Core.Interceptors;
using nGrpc.Common;
using System.Threading;
using System.Threading.Tasks;

namespace nGrpc.Client.Interceptors
{
    public class LoggerInterceptor : Interceptor
    {
        static int counter;
        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            int requestId = Interlocked.Increment(ref counter);
            string log = $@"({requestId}) Req: {context.Method.FullName}, Data: {request.ToJson()}";
            ClientLogger.Logger.LogInfo(log);

            AsyncUnaryCall<TResponse> call = base.AsyncUnaryCall(request, context, continuation);
            Task<TResponse> resAwaiter = call.ResponseAsync;
            Task.Run(async () =>
            {
                TResponse response = await resAwaiter;
                ClientLogger.Logger.LogInfo($"({requestId}), Res: {context.Method.FullName}, Data: {response.ToJson()}");
            });

            return call;
        }

        public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            return base.UnaryServerHandler(request, context, continuation);
        }
    }
}
