using nGrpc.Client;
using nGrpc.Client.GrpcServices;
using nGrpc.Common;
using System.Threading.Tasks;
using Xunit;

namespace nGrpc.IntegrationTest
{
    public class ChatServiceTest : IntegrationTestBase
    {
        [Fact]
        public async Task Chat_JoinRoom_Test()
        {
            (GrpcChannel grpcChannel, LoginRes loginRes) = await TestUtils.GetNewLoginedChannel();
            ChatGrpcService chatGrpcService = new ChatGrpcService(grpcChannel);

            JoinRoomReq req = new JoinRoomReq
            {
                RoomName = "adfasdfhds"
            };
            JoinRoomRes res = await chatGrpcService.JoinRoomRPC(req);

            Assert.NotNull(res);
        }

        [Fact]
        public async Task Chat_SendChat_Test()
        {
            (GrpcChannel grpcChannel, LoginRes loginRes) = await TestUtils.GetNewLoginedChannel();
            ChatGrpcService chatGrpcService = new ChatGrpcService(grpcChannel);
            string roomName = "fdgasdg";
            await chatGrpcService.JoinRoomRPC(new JoinRoomReq { RoomName = roomName });

            ChatMessage receivedChatMessage = null;
            chatGrpcService.OnChatReceived += chatMessage => receivedChatMessage = chatMessage;

            SendChatReq req = new SendChatReq
            {
                RoomName = roomName,
                Text = "iorasdnfzgasdgsdfg sfdg s"
            };
            SendChatRes res = await chatGrpcService.SendChatRPC(req);

            Assert.NotNull(res);
            await Task.Delay(100);
            Assert.NotNull(receivedChatMessage);
            Assert.Equal(loginRes.PlayerId, receivedChatMessage.PlayerId);
            Assert.Equal(roomName, receivedChatMessage.RoomName);
            Assert.Equal(req.Text, receivedChatMessage.Text);
        }
    }
}
