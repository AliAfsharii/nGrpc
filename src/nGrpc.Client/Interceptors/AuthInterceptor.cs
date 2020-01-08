using Grpc.Core;
using nGrpc.Common;

namespace nGrpc.Client.Interceptors
{
    public class AuthInterceptor
    {
        GrpcChannel _grpcChannel;
        public AuthInterceptor(GrpcChannel grpcChannel)
        {
            _grpcChannel = grpcChannel;
        }

        public Metadata AddHeader(Metadata source)
        {
            if (_grpcChannel.PlayerCredential != null)
                source.Add(HeaderKeys.credential.ToString(), _grpcChannel.PlayerCredential.ToJson());
            return source;
        }
    }
}