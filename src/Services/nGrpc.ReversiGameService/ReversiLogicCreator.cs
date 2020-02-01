using nGrpc.ServerCommon;
using System;

namespace nGrpc.ReversiGameService
{
    public class ReversiLogicCreator : IReversiLogicCreator
    {
        private readonly ReversiGameConfigs _reversiGameConfigs;
        private readonly ITimerProvider _timerProvider;
        private readonly IChatHub _chatHub;

        public ReversiLogicCreator(
            ReversiGameConfigs reversiGameConfigs,
            ITimerProvider timerProvider,
            IChatHub chatHub)
        {
            _reversiGameConfigs = reversiGameConfigs;
            _timerProvider = timerProvider;
            _chatHub = chatHub;
        }

        public IReversiLogic CreateLogic(int playerId1, string playerName1, int playerId2, string playerName2)
        {
            ITimer timer = _timerProvider.CreateTimer();

            string chatRoomName = Guid.NewGuid().ToString();
            _chatHub.JoinRoom(playerId1, chatRoomName);
            _chatHub.JoinRoom(playerId2, chatRoomName);

            ReversiLogic reversiLogic = new ReversiLogic(
                _reversiGameConfigs,
                timer,
                playerId1,
                playerName1,
                playerId2,
                playerName2,
                chatRoomName);
            return reversiLogic;
        }
    }
}
