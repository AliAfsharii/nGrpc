using System.Threading.Tasks;
using Xunit;
using nGrpc.ChatService;
using nGrpc.ServerCommon;
using NSubstitute;
using nGrpc.Common;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Logging;

namespace nGrpc.UnitTests.ChatServiceTests
{
    public class ChatHubTest
    {
        ChatHub _chatHub;
        IPubSubHub _pubSubHub;
        ChatConfigs _chatConfigs = new ChatConfigs
        {
            ChatGetLatestChatsCount = 5,
            ChatSaveIntervalInMilisec = 10 * 1000
        };
        ITime _time;

        public ChatHubTest()
        {
            _pubSubHub = Substitute.For<IPubSubHub>();
            _time = Substitute.For<ITime>();
            ChatRoom chatRoomFactory() => CreateChatRoom();
            _chatHub = new ChatHub(chatRoomFactory);
        }

        ChatRoom CreateChatRoom()
        {
            ILogger<ChatRoom> logger = Substitute.For<ILogger<ChatRoom>>();     
            IChatRepository chatRepository = Substitute.For<IChatRepository>();

            return new ChatRoom(logger, _time, _chatConfigs, chatRepository, _pubSubHub);
        }


        [Fact]
        public void GIVEN_ChatHub_WHEN_Call_Join_A_Room_And_Call_IsPlayerJoined_THEN_It_Should_Return_True()
        {
            // given
            ChatHub chatHub = _chatHub;
            int playerId = 46345;
            string roomName = "sfgfsfg";

            // when
            chatHub.Join(playerId, roomName);
            bool b = chatHub.IsPlayerJoined(playerId, roomName);

            // then
            Assert.True(b);
        }

        [Fact]
        public void GIVEN_ChatHub_WHEN_Dont_Call_Join_And_Call_IsPlayerJoined_THEN_It_Should_Return_False()
        {
            // given
            ChatHub chatHub = _chatHub;
            int playerId = 3456;
            string roomName = "sdfhsf";

            // when
            bool b = chatHub.IsPlayerJoined(playerId, roomName);

            // then
            Assert.False(b);
        }

        [Fact]
        public async Task GIVEN_ChatHub_With_A_Joined_Player_WHEN_Call_SendChat_THEN_PubSubHub_Publish_Should_Be_Called_Once()
        {
            // given
            ChatHub chatHub = _chatHub;
            IPubSubHub pubSubHub = _pubSubHub;
            ITime time = _time;

            int playerId = 784;
            string roomName = "wertwe";
            chatHub.Join(playerId, roomName);
            string chatText = "askdgh dshfa";

            // when
            await chatHub.SendChat(playerId, roomName, chatText);

            // then
            ChatSentMessage chatSentMessage = new ChatSentMessage
            {
                ChatRoomPlayersIds = new List<int> { playerId },
                ChatMessage = new ChatMessage
                {
                    Id = 1,
                    Date = time.UTCTime,
                    RoomName = roomName,
                    PlayerId = playerId,
                    Text = chatText,
                }
            };
            pubSubHub.Received(1).Publish(Arg.Is<ChatSentMessage>(message => message.ToJson() == chatSentMessage.ToJson()));
        }

        [Fact]
        public async Task GIVEN_ChatHub_Without_Joined_Player_WHEN_Call_SendChat_THEN_It_Should_Throw_PlayerHasNotJoinedToChatRoomException()
        {
            // given
            ChatHub chatHub = _chatHub;
            IPubSubHub pubSubHub = _pubSubHub;
            ITime time = _time;

            int playerId = 784;
            string roomName = "wertwe";
            string chatText = "askdgh dshfa";

            // when
            Exception exception = await Record.ExceptionAsync(() => chatHub.SendChat(playerId, roomName, chatText));

            // then
            Assert.NotNull(exception);
            Assert.IsType<PlayerHasNotJoinedToChatRoomException>(exception);
        }

        [Fact]
        public async Task GIVEN_ChatHub_With_Multiple_SentChats_WHEN_Call_GetLatestChats_THEN_It_Should_Return_N_SentChats_And_N_Is_In_Configs_chatGetLatestChatsCount()
        {
            // given
            ChatHub chatHub = _chatHub;
            IPubSubHub pubSubHub = _pubSubHub;
            ChatConfigs chatConfigs = _chatConfigs;

            chatConfigs.ChatGetLatestChatsCount = 4;
            int playerId = 784;
            string roomName = "dnxcvbx";
            chatHub.Join(playerId, roomName);
            string chatText = "askdgh dshfa";
            int sentChatsCount = 10;
            for (int i = 0; i < sentChatsCount; i++)
                await chatHub.SendChat(playerId, roomName, chatText + $"_{i}");

            // when
            int lastChatId = 4;
            List<ChatMessage> lastChats = await chatHub.GetLastChats(playerId, roomName, lastChatId);

            // then
            Assert.NotNull(lastChats);
            Assert.Equal(chatConfigs.ChatGetLatestChatsCount, lastChats.Count);
        }

        [Fact]
        public async Task GIVEN_ChatHub_With_Multiple_SentChats_WHEN_Call_GetLatestChats_With_WrongPlayerId_THEN_It_Should_Throw_PlayerHasNotJoinedToChatRoomException()
        {
            // given
            ChatHub chatHub = _chatHub;
            IPubSubHub pubSubHub = _pubSubHub;
            ChatConfigs chatConfigs = _chatConfigs;

            chatConfigs.ChatGetLatestChatsCount = 4;
            int playerId = 784;
            string roomName = "dnxcvbx";
            chatHub.Join(playerId, roomName);
            string chatText = "askdgh dshfa";
            int sentChatsCount = 10;
            for (int i = 0; i < sentChatsCount; i++)
                await chatHub.SendChat(playerId, roomName, chatText + $"_{i}");

            // when
            int lastChatId = 4;
            Exception exception = await Record.ExceptionAsync(() => chatHub.GetLastChats(playerId + 1, roomName, lastChatId));

            // then
            Assert.NotNull(exception);
            Assert.IsType<PlayerHasNotJoinedToChatRoomException>(exception);
        }
    }
}
