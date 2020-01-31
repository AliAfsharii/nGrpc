namespace nGrpc.ReversiGameService
{
    public interface IReversiLogic
    {
        ReversiGameData GetGameData(int playerId);
        bool IsPlayerInGame(int playerId);
        ReversiGameData PutDisk(int playerId, int row, int col);
    }
}