using nGrpc.ServerCommon;
using System;
using System.Collections.Generic;
using System.Linq;

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


        private bool CalculateNewMove(ReversiCellColor newDiskColor, int row, int col)
        {
            _cellColors[row, col] = newDiskColor;
            ReversiCellColor oppositeColor = GetOppositeColor(newDiskColor);

            List<bool> results = new List<bool>();
            results.Add(CalculateDirection(newDiskColor, row, col, 0, -1)); // left
            results.Add(CalculateDirection(newDiskColor, row, col, 0, 1)); // right
            results.Add(CalculateDirection(newDiskColor, row, col, 1, 0)); // down
            results.Add(CalculateDirection(newDiskColor, row, col, -1, 0)); // up
            results.Add(CalculateDirection(newDiskColor, row, col, 1, 1)); // down right
            results.Add(CalculateDirection(newDiskColor, row, col, 1, -1)); // down left
            results.Add(CalculateDirection(newDiskColor, row, col, -1, -1)); // up left
            results.Add(CalculateDirection(newDiskColor, row, col, -1, 1)); // up right

            return results.Any(n => n == true);
        }

        private bool CalculateDirection(ReversiCellColor newDiskColor, int row, int col, int rowStep, int colStep)
        {
            bool b = false;
            List<(int row, int col)> list = new List<(int row, int col)>();
            for (int y = col + colStep, x = row + rowStep; (y >= 0 && y <= 7) && (x >= 0 && x <= 7); y += colStep, x += rowStep)
            {
                if (_cellColors[x, y] == newDiskColor)
                {
                    b = true;
                    break;
                }
                if (_cellColors[x, y] == ReversiCellColor.Empty)
                    break;

                list.Add((x, y));
            }

            if (b == true)
                foreach ((int x, int y) in list)
                    _cellColors[x, y] = newDiskColor;

            return b;
        }

        void ChangeTurn()
        {
            _turnPlayerId = _turnPlayerId == _playerId1 ? _playerId2 : _playerId1;
        }


        //public

        public ReversiGameData GetGameData()
        {
            return CreateGameData();
        }

        public ReversiGameData PutDisk(int playerId, int row, int col)
        {
            if (playerId != _turnPlayerId)
                throw new WrongPlayerIdException($"PlayerId:{playerId}, ExpectedPlayerId:{_turnPlayerId}");

            ReversiCellColor playerColor = GetPlayerColor(playerId);
            bool b = CalculateNewMove(playerColor, row, col);
            if (b == false)
                throw new DiskOnWrongPositionException($"PlayerId:{playerId}, Row:{row}, Col:{col}");

            ChangeTurn();

            return CreateGameData();
        }

    }
}
