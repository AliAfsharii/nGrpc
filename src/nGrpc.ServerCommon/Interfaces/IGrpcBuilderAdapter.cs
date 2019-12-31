using Grpc.Core;

namespace nGrpc.ServerCommon
{
    public interface IGrpcBuilderAdapter
    {
        void AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, UnaryServerMethod<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class;

        void AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, ClientStreamingServerMethod<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class;

        void AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, ServerStreamingServerMethod<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class;

        void AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, DuplexStreamingServerMethod<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class;
    }
}
