using nGrpc.ServerCommon;

namespace nGrpc.ReversiGameService
{
    public class ReversiLogicCreator : IReversiLogicCreator
    {
        private readonly ReversiGameConfigs _reversiGameConfigs;
        private readonly ITimerProvider _timerProvider;

        public ReversiLogicCreator(
            ReversiGameConfigs reversiGameConfigs,
            ITimerProvider timerProvider)
        {
            _reversiGameConfigs = reversiGameConfigs;
            _timerProvider = timerProvider;
        }

        public IReversiLogic CreateLogic(int playerId1, string playerName1, int playerId2, string playerName2)
        {
            ITimer timer = _timerProvider.CreateTimer();

            ReversiLogic reversiLogic = new ReversiLogic(
                _reversiGameConfigs,
                timer,
                playerId1,
                playerName1,
                playerId2,
                playerName2
                );
            return reversiLogic;
        }
    }
}
