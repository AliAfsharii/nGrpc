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
    }
}
