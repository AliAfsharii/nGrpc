namespace nGrpc.Common
{
    public class JoinRoomReq
    {
        public string RoomName { get; set; }
    }
    public class JoinRoomRes
    {

    }

    public class SendChatReq
    {
        public string RoomName { get; set; }
        public string Text { get; set; }
    }
    public class SendChatRes
    {

    }
}
