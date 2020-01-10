using nGrpc.Sessions;
using System;
using Xunit;
using nGrpc.ServerCommon;
using NSubstitute;
using System.Threading;
using nGrpc.Common;
using System.Threading.Tasks;

namespace nGrpc.UnitTests.SessionTests
{
    public class SessionsManagerTest
    {
        int _playerId = 389465;
        ISessionsManager _sessionsManager;
        IProfileRepository _profileRepository;
        PlayerData _playerData;

        TimerMock _timerMock;
        SessionConfigs _sessionConfigs;

        public SessionsManagerTest()
        {
            ITimerProvider timerProvider = Substitute.For<ITimerProvider>();
            _timerMock = new TimerMock();
            timerProvider.CreateTimer().Returns(_timerMock);

            _sessionConfigs = new SessionConfigs { TimeoutInMilisec = 35312 };
            _profileRepository = Substitute.For<IProfileRepository>();

            _sessionsManager = new SessionsManager(timerProvider, _sessionConfigs, _profileRepository);
            _playerData = new PlayerData { Id = _playerId, SecretKey = Guid.NewGuid() };    
        }



        [Fact]
        public void GIVEN_SessionsManager_WHEN_Call_AddSession_And_GetPlayerData_THEN_It_Should_Return_Clone_Of_The_Same_PlayerData()
        {
            // given
            ISessionsManager sessionsManager = _sessionsManager;
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
            ISessionsManager sessionsManager = _sessionsManager;
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
            ISessionsManager sessionsManager = _sessionsManager;
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
            ISessionsManager sessionsManager = _sessionsManager;
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

        [Fact]
        public void GIVEN_SessionsManager_With_A_Session_WHEN_Call_HasSessionBySessionId_With_Correct_PlayerId_And_SessionId_THEN_It_Should_Return_True()
        {
            // given
            ISessionsManager sessionsManager = _sessionsManager;
            int playerId = _playerId;
            PlayerData playerData = _playerData;
            Guid sessionId = sessionsManager.AddSession(playerData);

            // when
            bool b = sessionsManager.HasSessionBySessionId(playerId, sessionId);

            // then
            Assert.True(b);
        }

        [Fact]
        public void GIVEN_SessionsManager_With_A_Session_WHEN_Call_HasSessionBySessionId_With_Correct_PlayerId_And_Wrong_SessionId_THEN_It_Should_Return_False()
        {
            // given
            ISessionsManager sessionsManager = _sessionsManager;
            int playerId = _playerId;
            PlayerData playerData = _playerData;
            Guid sessionId = sessionsManager.AddSession(playerData);

            // when
            bool b = sessionsManager.HasSessionBySessionId(playerId, Guid.NewGuid());

            // then
            Assert.False(b);
        }

        [Fact]
        public void GIVEN_SessionsManager_With_A_Session_WHEN_Call_HasSessionBySecretKey_With_Correct_PlayerId_And_SecretKey_THEN_It_Should_Return_True()
        {
            // given
            ISessionsManager sessionsManager = _sessionsManager;
            int playerId = _playerId;
            PlayerData playerData = _playerData;
            Guid expectedSessionId = sessionsManager.AddSession(playerData);

            // when
            bool b = sessionsManager.HasSessionBySecretKey(playerId, playerData.SecretKey, out Guid sessionId);

            // then
            Assert.True(b);
            Assert.Equal(expectedSessionId, sessionId);
        }

        [Fact]
        public void GIVEN_SessionsManager_With_A_Session_WHEN_Call_HasSessionBySecretKey_With_Correct_PlayerId_And_Wrong_SecretKey_THEN_It_Should_Return_False()
        {
            // given
            ISessionsManager sessionsManager = _sessionsManager;
            int playerId = _playerId;
            PlayerData playerData = _playerData;
            Guid sessionId = sessionsManager.AddSession(playerData);

            // when
            bool b = sessionsManager.HasSessionBySecretKey(playerId, Guid.NewGuid(), out _);

            // then
            Assert.False(b);
        }

        [Fact]
        public async Task GIVEN_SessionsManager_With_A_Session_WHEN_Call_ManipulatePlayerData_And_Change_CustomData_THEN_It_Should_Return_PlayerData_With_New_CustomData()
        {
            // given
            ISessionsManager sessionsManager = _sessionsManager;
            IProfileRepository profileRepository = _profileRepository;
            int playerId = _playerId;
            PlayerData playerData = _playerData;
            Guid sessionId = sessionsManager.AddSession(playerData);
            string customData = "custom";

            // when
            PlayerData pd = await sessionsManager.ManipulatePlayerData(playerId, p => p.CustomData = customData);

            // then
            Assert.Equal(customData, pd.CustomData);
            var p = sessionsManager.GetPlayerData(playerId);
            Assert.Equal(p.ToJson(), pd.ToJson());
            Assert.NotStrictEqual(playerData, pd);
            await profileRepository.Received(1).SavePlayerData(playerData);
        }
    }
}
