using Grpc.Core;
using nGrpc.ServerCommon;
using System.Collections.Generic;
using static Grpc.Core.ServerServiceDefinition;

namespace nGrpc.Grpc
{
    internal static class GrpcExtensions
    {
        internal static Builder AddServices(this Builder grpcBuilder, IEnumerable<IGrpcService> services)
        {
            IGrpcBuilderAdapter builder = new GrpcBuilderAdapter(grpcBuilder);
            foreach (var service in services)
                service.AddRpcMethods(builder);
            return grpcBuilder;
        }

        class GrpcBuilderAdapter : IGrpcBuilderAdapter
        {
            private readonly Builder builder;

            public GrpcBuilderAdapter(Builder builder)
            {
                this.builder = builder;
            }

            public void AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, UnaryServerMethod<TRequest, TResponse> handler)
                where TRequest : class
                where TResponse : class
            {
                builder.AddMethod(method, handler);
            }

            public void AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, ClientStreamingServerMethod<TRequest, TResponse> handler)
                where TRequest : class
                where TResponse : class
            {
                builder.AddMethod(method, handler);
            }

            public void AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, ServerStreamingServerMethod<TRequest, TResponse> handler)
                where TRequest : class
                where TResponse : class
            {
                builder.AddMethod(method, handler);
            }

            public void AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, DuplexStreamingServerMethod<TRequest, TResponse> handler)
                where TRequest : class
                where TResponse : class
            {
                builder.AddMethod(method, handler);
            }
        }
    }
}
