using nGrpc.Common;
using nGrpc.MatchMakeService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace nGrpc.UnitTests.MatchMakeTests
{
    public class RoomTest
    {
        Room _matchMakeRoom;
        MatchMakeConfigs _matchMakeConfigs = new MatchMakeConfigs
        {
            RoomCapacity = 2
        };

        public RoomTest()
        {
            _matchMakeRoom = new Room(_matchMakeConfigs);
        }

        [Fact]
        public void GIVEN_MatchMakeRoom_WHEN_Call_Join_THEN_One_PlayerId_Should_Be_In_Returned_Players()
        {
            // given
            Room matchMakeRoom = _matchMakeRoom;
            int playerId = 68464;
            string playerName = "asdfhsdsadf";

            // when
            (List<MatchMakePlayer> players, bool isRoomclosed) = matchMakeRoom.Join(playerId, playerName);

            // then
            Assert.False(isRoomclosed);
            Assert.Single(players);
            var p = players[0];
            Assert.Equal(playerId, p.Id);
            Assert.Equal(playerName, p.Name);
        }

        [Fact]
        public void GIVEN_MatchMakeRoom_WHEN_Call_Join_With_Two_Different_PlayerId_THEN_Two_PlayerId_Should_Be_In_Returned_Players()
        {
            // given
            Room matchMakeRoom = _matchMakeRoom;
            int playerId1 = 564;
            string playerName1 = "ghdfwr";
            int playerId2 = 45632;
            string playerName2 = "dfgdsdfgssf";

            // when
            matchMakeRoom.Join(playerId1, playerName1);
            (List<MatchMakePlayer> players, bool isRoomclosed) = matchMakeRoom.Join(playerId2, playerName2);

            // then
            Assert.True(isRoomclosed);
            Assert.Equal(2, players.Count);
            var p1 = players[0];
            Assert.Equal(playerId1, p1.Id);
            Assert.Equal(playerName1, p1.Name);
            var p2 = players[1];
            Assert.Equal(playerId2, p2.Id);
            Assert.Equal(playerName2, p2.Name);
        }

        [Fact]
        public void GIVEN_MatchMakeRoom_With_A_Joined_Player_WHEN_Call_Join_With_TheSame_Player_THEN_It_Should_Throw_PlayerIsAlreadyInRoomException()
        {
            // given
            Room matchMakeRoom = _matchMakeRoom;
            int playerId = 7846;
            matchMakeRoom.Join(playerId, "");

            // when
            Exception exception = Record.Exception(() => matchMakeRoom.Join(playerId, ""));

            // then
            Assert.NotNull(exception);
            Assert.IsType<PlayerIsAlreadyInRoomException>(exception);
        }

        [Fact]
        public void GIVEN_MatchMakeRoom_WHEN_Make_Room_Full_THEN_RoomIsClosed_Should_Be_True()
        {
            // given
            Room matchMakeRoom = _matchMakeRoom;
            int playerId = 345;
            MatchMakeConfigs matchMakeConfigs = _matchMakeConfigs;

            // when
            for (int i = 0; i < matchMakeConfigs.RoomCapacity - 1; i++)
                matchMakeRoom.Join(playerId + i, "");
            (List<MatchMakePlayer> players, bool isRoomclosed) = matchMakeRoom.Join(playerId + matchMakeConfigs.RoomCapacity - 1, "");

            // then
            Assert.True(isRoomclosed);
        }

        [Fact]
        public void GIVEN_MatchMakeRoom_Full_Of_Players_WHEN_Call_Join_With_New_PlayerId_THEN_It_Should_Throw_RoomIsClosedException()
        {
            // given
            Room matchMakeRoom = _matchMakeRoom;
            int playerId = 6584;
            for (int i = 0; i < _matchMakeConfigs.RoomCapacity; i++)
                matchMakeRoom.Join(playerId + i, "");

            // when
            Exception exception = Record.Exception(() => matchMakeRoom.Join(playerId + 100, ""));

            // then
            Assert.NotNull(exception);
            Assert.IsType<RoomIsClosedException>(exception);
        }

        [Fact]
        public void GIVEN_MatchMakeRoom_With_A_Player_WHEN_Call_Leave_With_The_PlayerId_THEN_PlayerId_Should_Not_Be_In_Returned_Players()
        {
            // given
            Room matchMakeRoom = _matchMakeRoom;
            int playerId = 7956;
            matchMakeRoom.Join(playerId, "");

            // when
            List<MatchMakePlayer> players = matchMakeRoom.Leave(playerId);

            // then
            Assert.Empty(players);
        }

        [Fact]
        public void GIVEN_MatchMakeRoom_With_A_Player_WHEN_Call_Leave_With_Wrong_PlayerId_THEN_It_Should_Throw_PlayerIsNotInRoomException()
        {
            // given
            Room matchMakeRoom = _matchMakeRoom;
            int playerId = 7956;
            matchMakeRoom.Join(playerId, "");

            // when
            Exception exception = Record.Exception(() => matchMakeRoom.Leave(playerId + 1));

            // then
            Assert.NotNull(exception);
            Assert.IsType<PlayerIsNotInRoomException>(exception);
        }

        [Fact]
        public void GIVEN_Closed_MatchMakeRoom_With_A_Player_WHEN_Call_Leave_THEN_It_Should_Throw_RoomIsClosedException()
        {
            // given
            Room matchMakeRoom = _matchMakeRoom;
            int playerId = 7956;
            matchMakeRoom.Join(playerId, "");
            matchMakeRoom.Join(playerId + 1, "");

            // when
            Exception exception = Record.Exception(() => matchMakeRoom.Leave(playerId));

            // then
            Assert.NotNull(exception);
            Assert.IsType<RoomIsClosedException>(exception);
        }

        [Fact]
        public void GIVEN_MatchMakeRoom_With_A_Player_WHEN_Call_GetPlayers_THEN_It_Should_Return_Player()
        {
            // given
            Room matchMakeRoom = _matchMakeRoom;
            int playerId = 7956;
            matchMakeRoom.Join(playerId, "");

            // when
            List<int> playerIds = matchMakeRoom.GetPlayers();

            // then
            Assert.Single(playerIds);
            Assert.Equal(playerId, playerIds[0]);
        }
    }
}
