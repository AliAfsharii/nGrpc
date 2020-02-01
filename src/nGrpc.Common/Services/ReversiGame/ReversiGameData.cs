namespace nGrpc.Common
{
    public enum ReversiCellColor
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
        public ReversiCellColor[,] CellColors { get; set; }
        public int TurnPlayerId { get; set; }
        public string ChatRoomName { get; set; }
    }
}
