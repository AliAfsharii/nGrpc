namespace nGrpc.ReversiGameService
{
    public enum ReversiCellState
    {
        Empty = 0,
        Black,
        White
    }



    public class ReversiGameData
    {
        public int PlayerId1 { get; set; }
        public string PlayerName1 { get; set; }
        public int PlayerId2 { get; set; }
        public string PlayerName2 { get; set; }
        public ReversiCellState[,] CellStates { get; set; }
        public int TurnPlayerId { get; set; }
    }
}
