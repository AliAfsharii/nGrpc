namespace nGrpc.ServerCommon
{
    public interface IGrpcService
    {
        void AddRpcMethods(IGrpcBuilderAdapter grpcBuilder);
    }
}
