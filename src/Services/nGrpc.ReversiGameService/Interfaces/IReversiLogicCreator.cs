namespace nGrpc.ReversiGameService
{
    public interface IReversiLogicCreator
    {
        IReversiLogic CreateLogic(int playerId1, string playerName1, int playerId2, string playerName2);
    }
}
