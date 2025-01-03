﻿using nGrpc.ReversiGameService;
using Xunit;
using nGrpc.Common;
using System;
using nGrpc.ServerCommon;
using NSubstitute;
using System.Threading;

namespace nGrpc.UnitTests.ReversiGameTests
{
    public class ReversiLogicTest
    {
        ReversiLogic _reversiLogic;
        int _playerId1 = 5634;
        string _playerName1 = "fgsdfhf pjgc";
        int _playerId2 = 8356;
        string _playerName2 = "opduhbcvx vj sdg";
        ITimer _timer;
        ReversiGameConfigs _reversiGameConfigs = new ReversiGameConfigs
        {
            TurnTimeInMilisec = 1342345
        };
        Action _timerCallback;

        public ReversiLogicTest()
        {
            _timer = Substitute.For<ITimer>();
            _timer.When(x => x.SetCallback(Arg.Any<Action>())).Do(x => _timerCallback = x.ArgAt<Action>(0));

            _reversiLogic = new ReversiLogic(_reversiGameConfigs, _timer, _playerId1, _playerName1, _playerId2, _playerName2, "ChatRoomName");
        }

        [Fact]
        public void GIVEN_ReversiLogic_WHEN_GetGameData_THEN_Return_GameData_With_Initial_State()
        {
            // given
            ReversiLogic reversiLogic = _reversiLogic;

            // when
            ReversiGameData gameData = reversiLogic.GetGameData(_playerId1);

            // then
            Assert.NotNull(gameData);
            Assert.Equal(_playerId1, gameData.PlayerId1);
            Assert.Equal(_playerName1, gameData.PlayerName1);
            Assert.Equal(_playerId2, gameData.PlayerId2);
            Assert.Equal(_playerName2, gameData.PlayerName2);
            Assert.Equal(_playerId1, gameData.TurnPlayerId);

            ReversiCellColor[,] expectedCellStates = new ReversiCellColor[8, 8];
            expectedCellStates[3, 3] = ReversiCellColor.White;
            expectedCellStates[3, 4] = ReversiCellColor.Black;
            expectedCellStates[4, 3] = ReversiCellColor.Black;
            expectedCellStates[4, 4] = ReversiCellColor.White;
            Assert.Equal(expectedCellStates.ToJson(), gameData.CellColors.ToJson());
        }

        [Fact]
        public void GIVEN_ReversiLogic_WHEN_Call_PutDisk_On_Right_Side_THEN_It_Should_Return_GameData_With_New_States()
        {
            // given
            ReversiLogic reversiLogic = _reversiLogic;
            int playerId = _playerId1;
            int row = 3;
            int col = 5;

            // when
            ReversiGameData gameData = reversiLogic.PutDisk(playerId, row, col);

            // then
            ReversiCellColor[,] expectedCellStates = new ReversiCellColor[8, 8];
            expectedCellStates[3, 3] = ReversiCellColor.White;
            expectedCellStates[3, 4] = ReversiCellColor.White;
            expectedCellStates[3, 5] = ReversiCellColor.White;
            expectedCellStates[4, 3] = ReversiCellColor.Black;
            expectedCellStates[4, 4] = ReversiCellColor.White;
            Assert.Equal(expectedCellStates.ToJson(), gameData.CellColors.ToJson());
        }

        [Fact]
        public void GIVEN_ReversiLogic_WHEN_Call_PutDisk_On_Left_Side_THEN_It_Should_Return_GameData_With_New_States()
        {
            // given
            ReversiLogic reversiLogic = _reversiLogic;
            int playerId = _playerId1;
            int row = 4;
            int col = 2;

            // when
            ReversiGameData gameData = reversiLogic.PutDisk(playerId, row, col);

            // then
            ReversiCellColor[,] expectedCellStates = new ReversiCellColor[8, 8];
            expectedCellStates[3, 3] = ReversiCellColor.White;
            expectedCellStates[3, 4] = ReversiCellColor.Black;
            expectedCellStates[4, 2] = ReversiCellColor.White;
            expectedCellStates[4, 3] = ReversiCellColor.White;
            expectedCellStates[4, 4] = ReversiCellColor.White;
            Assert.Equal(expectedCellStates.ToJson(), gameData.CellColors.ToJson());
        }

        [Fact]
        public void GIVEN_ReversiLogic_WHEN_Call_PutDisk_On_Up_Side_THEN_It_Should_Return_GameData_With_New_States()
        {
            // given
            ReversiLogic reversiLogic = _reversiLogic;
            int playerId = _playerId1;
            int row = 2;
            int col = 4;

            // when
            ReversiGameData gameData = reversiLogic.PutDisk(playerId, row, col);

            // then
            ReversiCellColor[,] expectedCellStates = new ReversiCellColor[8, 8];
            expectedCellStates[2, 4] = ReversiCellColor.White;
            expectedCellStates[3, 3] = ReversiCellColor.White;
            expectedCellStates[3, 4] = ReversiCellColor.White;
            expectedCellStates[4, 3] = ReversiCellColor.Black;
            expectedCellStates[4, 4] = ReversiCellColor.White;
            Assert.Equal(expectedCellStates.ToJson(), gameData.CellColors.ToJson());
        }

        [Fact]
        public void GIVEN_ReversiLogic_WHEN_Call_PutDisk_On_Down_Side_THEN_It_Should_Return_GameData_With_New_States()
        {
            // given
            ReversiLogic reversiLogic = _reversiLogic;
            int playerId = _playerId1;
            int row = 5;
            int col = 3;

            // when
            ReversiGameData gameData = reversiLogic.PutDisk(playerId, row, col);

            // then
            ReversiCellColor[,] expectedCellStates = new ReversiCellColor[8, 8];
            expectedCellStates[3, 3] = ReversiCellColor.White;
            expectedCellStates[3, 4] = ReversiCellColor.Black;
            expectedCellStates[4, 3] = ReversiCellColor.White;
            expectedCellStates[4, 4] = ReversiCellColor.White;
            expectedCellStates[5, 3] = ReversiCellColor.White;
            Assert.Equal(expectedCellStates.ToJson(), gameData.CellColors.ToJson());
        }

        [Fact]
        public void GIVEN_ReversiLogic_WHEN_Call_PutDisk_On_UpRight_Side_THEN_It_Should_Return_GameData_With_New_States()
        {
            // given
            ReversiLogic reversiLogic = _reversiLogic;
            reversiLogic.PutDisk(_playerId1, 2, 4);

            int playerId = _playerId2;
            int row = 2;
            int col = 5;

            // when
            ReversiGameData gameData = reversiLogic.PutDisk(playerId, row, col);

            // then
            ReversiCellColor[,] expectedCellStates = new ReversiCellColor[8, 8];
            expectedCellStates[2, 4] = ReversiCellColor.White;
            expectedCellStates[2, 5] = ReversiCellColor.Black;
            expectedCellStates[3, 3] = ReversiCellColor.White;
            expectedCellStates[3, 4] = ReversiCellColor.Black;
            expectedCellStates[4, 3] = ReversiCellColor.Black;
            expectedCellStates[4, 4] = ReversiCellColor.White;
            Assert.Equal(expectedCellStates.ToJson(), gameData.CellColors.ToJson());
        }

        [Fact]
        public void GIVEN_ReversiLogic_WHEN_Call_PutDisk_On_UpLeft_Side_THEN_It_Should_Return_GameData_With_New_States()
        {
            // given
            ReversiLogic reversiLogic = _reversiLogic;
            reversiLogic.PutDisk(_playerId1, 4, 2);
            reversiLogic.PutDisk(_playerId2, 3, 2);

            int playerId = _playerId1;
            int row = 2;
            int col = 2;

            // when
            ReversiGameData gameData = reversiLogic.PutDisk(playerId, row, col);

            // then
            ReversiCellColor[,] expectedCellStates = new ReversiCellColor[8, 8];
            expectedCellStates[2, 2] = ReversiCellColor.White;
            expectedCellStates[3, 2] = ReversiCellColor.White;
            expectedCellStates[3, 3] = ReversiCellColor.White;
            expectedCellStates[3, 4] = ReversiCellColor.Black;
            expectedCellStates[4, 2] = ReversiCellColor.White;
            expectedCellStates[4, 3] = ReversiCellColor.White;
            expectedCellStates[4, 4] = ReversiCellColor.White;
            Assert.Equal(expectedCellStates.ToJson(), gameData.CellColors.ToJson());
        }

        [Fact]
        public void GIVEN_ReversiLogic_WHEN_Call_PutDisk_On_DownLeft_Side_THEN_It_Should_Return_GameData_With_New_States()
        {
            // given
            ReversiLogic reversiLogic = _reversiLogic;
            reversiLogic.PutDisk(_playerId1, 4, 2);

            int playerId = _playerId2;
            int row = 5;
            int col = 2;

            // when
            ReversiGameData gameData = reversiLogic.PutDisk(playerId, row, col);

            // then
            ReversiCellColor[,] expectedCellStates = new ReversiCellColor[8, 8];
            expectedCellStates[3, 3] = ReversiCellColor.White;
            expectedCellStates[3, 4] = ReversiCellColor.Black;
            expectedCellStates[4, 2] = ReversiCellColor.White;
            expectedCellStates[4, 3] = ReversiCellColor.Black;
            expectedCellStates[4, 4] = ReversiCellColor.White;
            expectedCellStates[5, 2] = ReversiCellColor.Black;
            Assert.Equal(expectedCellStates.ToJson(), gameData.CellColors.ToJson());
        }

        [Fact]
        public void GIVEN_ReversiLogic_WHEN_Call_PutDisk_On_DownRight_Side_THEN_It_Should_Return_GameData_With_New_States()
        {
            // given
            ReversiLogic reversiLogic = _reversiLogic;
            reversiLogic.PutDisk(_playerId1, 3, 5);
            reversiLogic.PutDisk(_playerId2, 4, 5);

            int playerId = _playerId1;
            int row = 5;
            int col = 5;

            // when
            ReversiGameData gameData = reversiLogic.PutDisk(playerId, row, col);

            // then
            ReversiCellColor[,] expectedCellStates = new ReversiCellColor[8, 8];
            expectedCellStates[3, 3] = ReversiCellColor.White;
            expectedCellStates[3, 4] = ReversiCellColor.White;
            expectedCellStates[3, 5] = ReversiCellColor.White;
            expectedCellStates[4, 3] = ReversiCellColor.Black;
            expectedCellStates[4, 4] = ReversiCellColor.White;
            expectedCellStates[4, 5] = ReversiCellColor.White;
            expectedCellStates[5, 5] = ReversiCellColor.White;
            Assert.Equal(expectedCellStates.ToJson(), gameData.CellColors.ToJson());
        }

        [Fact]
        public void GIVEN_ReversiLogic_WHEN_Call_PutDisk_On_WrongPosition_THEN_It_Should_Throw_DiskOnWrongPositionException()
        {
            // given
            ReversiLogic reversiLogic = _reversiLogic;

            int playerId = _playerId1;
            int row = 0;
            int col = 0;

            // when
            Exception exception = Record.Exception(() => reversiLogic.PutDisk(playerId, row, col));

            // then
            Assert.NotNull(exception);
            Assert.IsType<DiskOnWrongPositionException>(exception);
        }

        [Fact]
        public void GIVEN_ReversiLogic_WHEN_Call_PutDisk_With_WrongPlayerId_THEN_It_Should_Throw_WrongPlayerIdException()
        {
            // given
            ReversiLogic reversiLogic = _reversiLogic;
            int wrongPlayerId = _playerId1 + _playerId2;

            // when
            Exception exception = Record.Exception(() => reversiLogic.PutDisk(wrongPlayerId, 0, 0));

            // then
            Assert.NotNull(exception);
            Assert.IsType<WrongPlayerIdException>(exception);
        }

        [Fact]
        public void GIVEN_ReversiLogic_THEN_Timer_SetCallAback_And_Change_Should_Be_Called_Once()
        {
            // given
            ReversiLogic reversiLogic = _reversiLogic;
            ITimer timer = _timer;
            var config = _reversiGameConfigs;

            // when

            // then
            timer.Received(1).SetCallback(Arg.Any<Action>());
            timer.Received(1).Change(config.TurnTimeInMilisec, Timeout.Infinite);
        }

        [Fact]
        public void GIVEN_ReversiLogic_WHEN_Call_Timer_Callback_THEN_Turn_Should_Be_Changed()
        {
            // given
            ReversiLogic reversiLogic = _reversiLogic;
            Action timerCallback = _timerCallback;

            // when
            timerCallback();

            // then
            ReversiGameData gameData = reversiLogic.GetGameData(_playerId1);
            Assert.Equal(_playerId2, gameData.TurnPlayerId);
        }

        [Fact]
        public void GIVEN_ReversiLogic_WHEN_Call_PutDisk_With_A_Filled_Position_THEN_It_Should_Throw_CellIsFilledException()
        {
            // given
            ReversiLogic reversiLogic = _reversiLogic;
            int playerId = _playerId1;
            int row = 3;
            int col = 3;

            // when
            Exception exception = Record.Exception(() => reversiLogic.PutDisk(playerId, row, col));

            // then
            Assert.NotNull(exception);
            Assert.IsType<CellIsFilledException>(exception);
        }

        [Fact]
        public void GIVEN_ReversiLogic_WHEN_THEN_()
        {
            // given
            ReversiLogic reversiLogic = _reversiLogic;

            // when


            // then

        }
    }
}
