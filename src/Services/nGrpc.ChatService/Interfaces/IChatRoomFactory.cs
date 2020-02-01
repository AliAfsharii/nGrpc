namespace nGrpc.ChatService
{
    public interface IChatRoomFactory
    {
        ChatRoom CreateNewChatRoom(string roomName);
    }
}
