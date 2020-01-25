﻿using Microsoft.Extensions.Logging;
using nGrpc.Common;
using nGrpc.MatchMakeService;
using nGrpc.ServerCommon;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace nGrpc.UnitTests.MatchMakeTests
{
    public class MatchMakeRoomTest
    {
        MatchMakeRoom _matchMakeRoom;
        MatchMakeConfigs _matchMakeConfigs = new MatchMakeConfigs();

        public MatchMakeRoomTest()
        {
            _matchMakeRoom = new MatchMakeRoom(_matchMakeConfigs);
        }

        [Fact]
        public async Task GIVEN_MatchMakeRoom_WHEN_Call_Join_THEN_One_PlayerId_Should_Be_In_Returned_Players()
        {
            // given
            MatchMakeRoom matchMakeRoom = _matchMakeRoom;
            int playerId = 68464;
            string playerName = "asdfhsdsadf";

            // when
            (List<MatchMakePlayer> players, bool isRoomclosed) = await matchMakeRoom.Join(playerId, playerName);

            // then
            Assert.False(isRoomclosed);
            Assert.Single(players);
            var p = players[0];
            Assert.Equal(playerId, p.Id);
            Assert.Equal(playerName, p.Name);
        }

        [Fact]
        public async Task GIVEN_MatchMakeRoom_WHEN_Call_Join_With_Two_Different_PlayerId_THEN_Two_PlayerId_Should_Be_In_Returned_Players()
        {
            // given
            MatchMakeRoom matchMakeRoom = _matchMakeRoom;
            int playerId1 = 564;
            string playerName1 = "ghdfwr";
            int playerId2 = 45632;
            string playerName2 = "dfgdsdfgssf";

            // when
            await matchMakeRoom.Join(playerId1, playerName1);
            (List<MatchMakePlayer> players, bool isRoomclosed) = await matchMakeRoom.Join(playerId2, playerName2);

            // then
            Assert.False(isRoomclosed);
            Assert.Equal(2, players.Count);
            var p1 = players[0];
            Assert.Equal(playerId1, p1.Id);
            Assert.Equal(playerName1, p1.Name);
            var p2 = players[1];
            Assert.Equal(playerId2, p2.Id);
            Assert.Equal(playerName2, p2.Name);
        }

        [Fact]
        public async Task GIVEN_MatchMakeRoom_With_A_Joined_Player_WHEN_Call_Join_With_TheSame_Player_THEN_It_Should_Throw_PlayerIsAlreadyInRoomException()
        {
            // given
            MatchMakeRoom matchMakeRoom = _matchMakeRoom;
            int playerId = 7846;
            await matchMakeRoom.Join(playerId, "");

            // when
            Exception exception = await Record.ExceptionAsync(() => matchMakeRoom.Join(playerId, ""));

            // then
            Assert.NotNull(exception);
            Assert.IsType<PlayerIsAlreadyInRoomException>(exception);
        }

        [Fact]
        public async Task GIVEN_MatchMakeRoom_WHEN_Make_Room_Full_THEN_RoomIsClosed_Should_Be_True()
        {
            // given
            MatchMakeRoom matchMakeRoom = _matchMakeRoom;
            int playerId = 345;
            MatchMakeConfigs matchMakeConfigs = _matchMakeConfigs;
            matchMakeConfigs.RoomCapacity = 2;

            //int matchId = 984658743;
            //IMatchProvider matchProvider = _matchProvider;
            //List<int> playerIds = Enumerable.Range(0, matchMakeConfigs.RoomCapacity).Select(n => playerId + n).ToList();
            //matchProvider.CreateMatch(Arg.Is<List<int>>(l => l.ToJson() == playerIds.ToJson())).Returns(matchId);

            // when
            for (int i = 0; i < matchMakeConfigs.RoomCapacity - 1; i++)
                await matchMakeRoom.Join(playerId + i, "");
            (List<MatchMakePlayer> players, bool isRoomclosed) = await matchMakeRoom.Join(playerId + matchMakeConfigs.RoomCapacity - 1, "");

            // then
            Assert.True(isRoomclosed);
        }

        [Fact]
        public async Task GIVEN_MatchMakeRoom_Full_Of_Players_WHEN_Call_Join_With_New_PlayerId_THEN_It_Should_Throw_RoomIsClosedException()
        {
            // given
            MatchMakeRoom matchMakeRoom = _matchMakeRoom;
            int playerId = 6584;
            MatchMakeConfigs matchMakeConfigs = _matchMakeConfigs;
            matchMakeConfigs.RoomCapacity = 2;
            for (int i = 0; i < matchMakeConfigs.RoomCapacity; i++)
                await matchMakeRoom.Join(playerId + i, "");

            // when
            Exception exception = await Record.ExceptionAsync(() => matchMakeRoom.Join(playerId + 100, ""));

            // then
            Assert.NotNull(exception);
            Assert.IsType<RoomIsClosedException>(exception);
        }

        [Fact]
        public async Task GIVEN_MatchMakeRoom_With_A_Player_WHEN_Call_Leave_With_The_PlayerId_THEN_PlayerId_Should_Not_Be_In_Returned_Players()
        {
            // given
            MatchMakeRoom matchMakeRoom = _matchMakeRoom;
            int playerId = 7956;
            await matchMakeRoom.Join(playerId, "");

            // when
            List<MatchMakePlayer> players = await matchMakeRoom.Leave(playerId);

            // then
            Assert.Empty(players);
        }

        [Fact]
        public async Task GIVEN_MatchMakeRoom_With_A_Player_WHEN_Call_Leave_With_Wrong_PlayerId_THEN_It_Should_Throw_PlayerIsNotInRoomException()
        {
            // given
            MatchMakeRoom matchMakeRoom = _matchMakeRoom;
            int playerId = 7956;
            await matchMakeRoom.Join(playerId, "");

            // when
            Exception exception = await Record.ExceptionAsync(() => matchMakeRoom.Leave(playerId + 1));

            // then
            Assert.NotNull(exception);
            Assert.IsType<PlayerIsNotInRoomException>(exception);
        }
    }
}
