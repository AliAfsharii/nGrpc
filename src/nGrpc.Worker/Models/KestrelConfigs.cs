namespace nGrpc.Worker
{
    public class KestrelConfigs
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public int MaxConcurrentConnections { get; set; }
    }
}
