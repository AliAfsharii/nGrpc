namespace nGrpc.Common
{
    public enum ServerEventType { }

    public class ServerEventReq
    {

    }
    public class ServerEventRes
    {
        public ServerEventType ServerEventType { get; set; }
        public string JsonData { get; set; }
    }
}
