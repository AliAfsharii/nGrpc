using nGrpc.ServerCommon;
using System;
using System.Collections.Generic;

namespace nGrpc.ReversiGameService
{
    public class ReversiLogic
    {
        private readonly int _playerId1;
        private readonly string _playerName1;
        private readonly int _playerId2;
        private readonly string _playerName2;
        private readonly ReversiCellColor[,] _cellColors;
        private int _turnPlayerId;

        public ReversiLogic(int playerId1, string playerName1, int playerId2, string playerName2)
        {
            _playerId1 = playerId1;
            _playerName1 = playerName1;
            _playerId2 = playerId2;
            _playerName2 = playerName2;

            _cellColors = new ReversiCellColor[8, 8];
            _cellColors[3, 3] = ReversiCellColor.White;
            _cellColors[3, 4] = ReversiCellColor.Black;
            _cellColors[4, 3] = ReversiCellColor.Black;
            _cellColors[4, 4] = ReversiCellColor.White;

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
                CellColors = _cellColors.CloneByMessagePack(),
                TurnPlayerId = _playerId1
            };
            return gameData;
        }

        private ReversiCellColor GetPlayerColor(int playerId)
        {
            return playerId == _playerId1 ? ReversiCellColor.White : ReversiCellColor.Black;
        }

        private ReversiCellColor GetOppositeColor(ReversiCellColor color)
        {
            if (color == ReversiCellColor.Empty)
                throw new Exception("Empty does not have opposite color.");
            return color == ReversiCellColor.White ? ReversiCellColor.Black : ReversiCellColor.White;
        }


        private void CalculateNewMove(ReversiCellColor newDiskColor, int rowNum, int colNum)
        {
            _cellColors[rowNum, colNum] = newDiskColor;
            ReversiCellColor oppositeColor = GetOppositeColor(newDiskColor);

            // check left cells\
            bool b = false;
            List<(int row, int col)> list = new List<(int row, int col)>();
            for (int i = colNum - 1; i >= 0; i--)
            {
                if (_cellColors[rowNum, i] == newDiskColor)
                {
                    b = true;
                    break;
                }
                if (_cellColors[rowNum, i] == ReversiCellColor.Empty)
                    break;

                list.Add((rowNum, i));
            }

            if (b == true)
                foreach ((int x, int y) in list)
                    _cellColors[x, y] = newDiskColor;
        }



        //public

        public ReversiGameData GetGameData()
        {
            return CreateGameData();
        }

        public ReversiGameData PutDisk(int playerId, int rowNum, int colNum)
        {
            ReversiCellColor playerColor = GetPlayerColor(playerId);
            CalculateNewMove(playerColor, rowNum, colNum);
            return CreateGameData();
        }

    }
}
