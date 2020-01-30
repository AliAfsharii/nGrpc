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

            ReversiCellColor[,] expectedCellStates = new ReversiCellColor[8, 8];
            expectedCellStates[3, 3] = ReversiCellColor.White;
            expectedCellStates[3, 4] = ReversiCellColor.Black;
            expectedCellStates[4, 3] = ReversiCellColor.Black;
            expectedCellStates[4, 4] = ReversiCellColor.White;
            Assert.Equal(expectedCellStates.ToJson(), gameData.CellColors.ToJson());
        }

        [Fact]
        public void GIVEN_ReversiLogic_WHEN_Call_PutDisk_With_CorrectPlayerId_And_CorrectPosition_THEN_It_Should_Return_GameData_With_New_States()
        {
            // given
            ReversiLogic reversiLogic = _reversiLogic;
            int playerId = _playerId1;
            int rowNum = 3;
            int colNum = 5;

            // when
            ReversiGameData gameData = reversiLogic.PutDisk(playerId, rowNum, colNum);

            // then
            ReversiCellColor[,] expectedCellStates = new ReversiCellColor[8, 8];
            expectedCellStates[3, 3] = ReversiCellColor.White;
            expectedCellStates[3, 4] = ReversiCellColor.White;
            expectedCellStates[4, 3] = ReversiCellColor.Black;
            expectedCellStates[4, 4] = ReversiCellColor.White;
            expectedCellStates[3, 5] = ReversiCellColor.White;
            Assert.Equal(expectedCellStates.ToJson(), gameData.CellColors.ToJson());
        }
    }
}
