﻿using nGrpc.Client;
using nGrpc.Client.GrpcServices;
using nGrpc.Common;
using System;
using System.Linq;
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

        [Fact]
        public async Task Chat_LeaveRoom_Test()
        {
            (GrpcChannel grpcChannel, LoginRes loginRes) = await TestUtils.GetNewLoginedChannel();
            ChatGrpcService chatGrpcService = new ChatGrpcService(grpcChannel);
            string roomName = "fdgasdg";
            await chatGrpcService.JoinRoomRPC(new JoinRoomReq { RoomName = roomName });

            await chatGrpcService.LeaveRoomRPC(new LeaveRoomReq { RoomName = roomName });
            SendChatReq req = new SendChatReq
            {
                RoomName = roomName,
                Text = "iorasdnfzgasdgsdfg sfdg s"
            };
            Exception exception = await Record.ExceptionAsync(() => chatGrpcService.SendChatRPC(req));

            Assert.NotNull(exception);
        }

        [Fact]
        public async Task Chat_GetLastChats_Test()
        {
            (GrpcChannel grpcChannel, LoginRes loginRes) = await TestUtils.GetNewLoginedChannel();
            ChatGrpcService chatGrpcService = new ChatGrpcService(grpcChannel);
            string roomName = "fdgasdg";
            string chatText = "uidyfisdfnasdhguiu";
            await chatGrpcService.JoinRoomRPC(new JoinRoomReq { RoomName = roomName });

            var chatConfig = GetServiceFromProvider<ChatService.ChatConfigs>();

            for (int i = 0; i < chatConfig.ChatGetLastChatsCount + 5; i++)
            {
                SendChatReq req = new SendChatReq
                {
                    RoomName = roomName,
                    Text = chatText + "_" + i
                };
                await chatGrpcService.SendChatRPC(req);
            }

            GetLastChatsReq getLastChatsReq = new GetLastChatsReq { RoomName = roomName, LastChatId = -1 };
            GetLastChatsRes getLastChatsRes = await chatGrpcService.GetLastChatsRPC(getLastChatsReq);

            Assert.NotNull(getLastChatsRes);
            Assert.Equal(chatConfig.ChatGetLastChatsCount, getLastChatsRes.ChatMessages.Count);
            Assert.Equal($"{chatText}_{chatConfig.ChatGetLastChatsCount + 4}", getLastChatsRes.ChatMessages.Last().Text);
        }
    }
}