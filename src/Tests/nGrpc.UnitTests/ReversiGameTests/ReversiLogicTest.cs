using nGrpc.ReversiGameService;
using Xunit;
using nGrpc.Common;

namespace nGrpc.UnitTests.ReversiGameTests
{
    public class ReversiLogicTest
    {
        ReversiLogic _reversiLogic;
        int _playerId1 = 5634;
        string _playerName1 = "fgsdfhf pjgc";
        int _playerId2 = 8356;
        string _playerName2 = "opduhbcvx vj sdg";

        public ReversiLogicTest()
        {
            _reversiLogic = new ReversiLogic(_playerId1, _playerName1, _playerId2, _playerName2);
        }

        [Fact]
        public void GIVEN_ReversiLogic_WHEN_GetGameData_THEN_Return_GameData_With_Initial_State()
        {
            // given
            ReversiLogic reversiLogic = _reversiLogic;

            // when
            ReversiGameData gameData = reversiLogic.GetGameData();

            // then
            Assert.NotNull(gameData);
            Assert.Equal(_playerId1, gameData.PlayerId1);
            Assert.Equal(_playerName1, gameData.PlayerName1);
            Assert.Equal(_playerId2, gameData.PlayerId2);
            Assert.Equal(_playerName2, gameData.PlayerName2);
            Assert.Equal(_playerId1, gameData.TurnPlayerId);

            ReversiCellState[,] expectedCellStates = new ReversiCellState[8, 8];
            expectedCellStates[4, 4] = ReversiCellState.White;
            expectedCellStates[4, 5] = ReversiCellState.Black;
            expectedCellStates[5, 4] = ReversiCellState.Black;
            expectedCellStates[5, 5] = ReversiCellState.White;
            Assert.Equal(expectedCellStates.ToJson(), gameData.CellStates.ToJson());
        }

        [Fact]
        public void GIVEN_ReversiLogic_WHEN_Call_PutDisk_With_CorrectPlayerId_And_CorrectPosition_THEN_It_Should_Return_GameData_With_New_States()
        {
            // given
            ReversiLogic reversiLogic = _reversiLogic;
            int playerId = _playerId1;
            int rowNum = 4;
            int colNum = 6;

            // when
            ReversiGameData gameData = reversiLogic.PutDisk(playerId, rowNum, colNum);

            // then
            ReversiCellState[,] expectedCellStates = new ReversiCellState[8, 8];
            expectedCellStates[4, 4] = ReversiCellState.White;
            expectedCellStates[4, 5] = ReversiCellState.White;
            expectedCellStates[5, 4] = ReversiCellState.Black;
            expectedCellStates[5, 5] = ReversiCellState.White;
            expectedCellStates[4, 6] = ReversiCellState.White;
            Assert.Equal(expectedCellStates.ToJson(), gameData.CellStates.ToJson());
        }
    }
}
