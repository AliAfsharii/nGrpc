namespace nGrpc.Common
{
    public class GetGameDataReq
    {
        public int GameId { get; set; }
    }
    public class GetGameDataRes
    {
        public ReversiGameData GameData { get; set; }
    }

    public class PutDiskReq
    {
        public int GameId { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
    }
    public class PutDiskRes
    {
        public ReversiGameData GameData { get; set; }
    }
}
