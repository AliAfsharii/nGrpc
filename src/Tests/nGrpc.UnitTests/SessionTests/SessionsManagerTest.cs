using nGrpc.Sessions;
using System;
using Xunit;
using nGrpc.ServerCommon;
using NSubstitute;
using System.Threading;

namespace nGrpc.UnitTests.SessionTests
{
    public class SessionsManagerTest
    {
        int _playerId = 389465;
        SessionsManager _sessionsManager;
        PlayerData _playerData;

        TimerMock _timerMock;
        ITimerProvider _timerProvider;
        SessionConfigs _sessionConfigs;

        public SessionsManagerTest()
        {
            _timerProvider = Substitute.For<ITimerProvider>();
            _timerMock = new TimerMock();
            _timerProvider.CreateTimer().Returns(_timerMock);

            _sessionConfigs = new SessionConfigs { TimeoutInMilisec = 35312 };

            _sessionsManager = new SessionsManager(_timerProvider, _sessionConfigs);
            _playerData = new PlayerData { Id = _playerId, SecretKey = Guid.NewGuid() };    
        }



        [Fact]
        public void GIVEN_SessionsManager_WHEN_Call_AddSession_And_GetPlayerData_THEN_It_Should_Return_Clone_Of_The_Same_PlayerData()
        {
            // given
            SessionsManager sessionsManager = _sessionsManager;
            int playerId = _playerId;
            PlayerData playerData = _playerData;

            // when
            sessionsManager.AddSession(playerData);
            PlayerData pd = sessionsManager.GetPlayerData(playerId);

            // then
            Assert.NotStrictEqual(playerData, pd);
            Assert.Equal(playerData.ToJson(), pd.ToJson());
        }

        [Fact]
        public void GIVEN_SessionsManager_With_A_Session_WHEN_Timeout_Is_Reached_THEN_It_Should_Not_Have_The_PlayerData()
        {
            // given
            SessionsManager sessionsManager = _sessionsManager;
            int playerId = _playerId;
            PlayerData playerData = _playerData;
            sessionsManager.AddSession(playerData);
            TimerMock timerMock = _timerMock;

            // when
            timerMock.Callback();

            // then
            Exception exception = Record.Exception(() => sessionsManager.GetPlayerData(playerId));
            Assert.NotNull(exception);
            Assert.IsType<ThereIsNoPlayerDataForSuchPlayerException>(exception);
        }

        [Fact]
        public void GIVEN_SessionsManager_WHEN_Call_AddSession_THEN_Timer_Change_Sould_Be_Called_Once_With_Config_Timeout()
        {
            // given
            SessionsManager sessionsManager = _sessionsManager;
            PlayerData playerData = _playerData;
            TimerMock timerMock = _timerMock;
            SessionConfigs sessionConfigs = _sessionConfigs;

            // when
            sessionsManager.AddSession(playerData);

            // then
            timerMock.SpyTimer.Received(1).Change(sessionConfigs.TimeoutInMilisec, Timeout.Infinite);
        }

        [Fact]
        public void GIVEN_SessionsManager_With_A_Session_WHEN_Call_ResetTimer_THEN_Timer_Change_Sould_Be_Called_Once_With_Config_Timeout()
        {
            // given
            SessionsManager sessionsManager = _sessionsManager;
            int playerId = _playerId;
            PlayerData playerData = _playerData;
            TimerMock timerMock = _timerMock;
            SessionConfigs sessionConfigs = _sessionConfigs;
            sessionsManager.AddSession(playerData);
            timerMock.SpyTimer.ClearReceivedCalls();

            // when
            sessionsManager.ResetTimer(playerId);

            // then
            timerMock.SpyTimer.Received(1).Change(sessionConfigs.TimeoutInMilisec, Timeout.Infinite);
        }
    }
}
