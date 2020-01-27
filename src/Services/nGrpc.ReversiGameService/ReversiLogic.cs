using nGrpc.ServerCommon;
using System;

namespace nGrpc.ReversiGameService
{
    public class ReversiLogic
    {
        private readonly int _playerId1;
        private readonly string _playerName1;
        private readonly int _playerId2;
        private readonly string _playerName2;
        private readonly ReversiCellState[,] _cellStates;
        private int _turnPlayerId;

        public ReversiLogic(int playerId1, string playerName1, int playerId2, string playerName2)
        {
            _playerId1 = playerId1;
            _playerName1 = playerName1;
            _playerId2 = playerId2;
            _playerName2 = playerName2;

            _cellStates = new ReversiCellState[8, 8];
            _cellStates[4, 4] = ReversiCellState.White;
            _cellStates[4, 5] = ReversiCellState.Black;
            _cellStates[5, 4] = ReversiCellState.Black;
            _cellStates[5, 5] = ReversiCellState.White;

            _turnPlayerId = playerId1;
        }


        // private

        private ReversiGameData CreateGameData()
        {
            ReversiGameData gameData = new ReversiGameData
            {
                PlayerId1 = _playerId1,
                PlayerName1 = _playerName1,
                PlayerId2 = _playerId2,
                PlayerName2 = _playerName2,
                CellStates = _cellStates.CloneByMessagePack(),
                TurnPlayerId = _playerId1
            };
            return gameData;
        }

        private ReversiCellState GetPlayerColor(int playerId)
        {
            return playerId == _playerId1 ? ReversiCellState.White : ReversiCellState.Black;
        }

        //public

        public ReversiGameData GetGameData()
        {
            return CreateGameData();
        }

        public ReversiGameData PutDisk(int playerId, int rowNum, int colNum)
        {
            ReversiCellState playerColor = GetPlayerColor(playerId);
            _cellStates[rowNum, colNum] = playerColor;


            return CreateGameData();
        }

    }
}
