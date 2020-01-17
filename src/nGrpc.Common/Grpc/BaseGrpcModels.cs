namespace nGrpc.Common
{
    public enum ServerEventType
    {
        Test, Chat
    }

    public class ServerEventReq
    {

    }
    public class ServerEventRes
    {
        public ServerEventType ServerEventType { get; set; }
        public string CustomData { get; set; }
    }

    public class ServerEventTestReq
    {
        public string CustomData { get; set; }
    }
    public class ServerEventTestRes { }
}
