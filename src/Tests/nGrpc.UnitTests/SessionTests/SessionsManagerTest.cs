using nGrpc.Sessions;
using System;
using System.Threading.Tasks;
using Xunit;
using nGrpc.ServerCommon;

namespace nGrpc.UnitTests.SessionTests
{
    public class SessionsManagerTest
    {
        [Fact]
        public void GIVEN_SessionsManager_WHEN_Call_AddSession_And_GetPlayerData_THEN_It_Should_Return_Clone_Of_The_Same_PlayerData()
        {
            // given
            SessionsManager sessionsManager = new SessionsManager();
            int playerId = 56234;
            PlayerData givenPlayerData = new PlayerData { Id = playerId, SecretKey = Guid.NewGuid() };

            // when
            sessionsManager.AddSession(givenPlayerData);
            PlayerData playerData = sessionsManager.GetPlayerData(playerId);

            // then
            Assert.NotStrictEqual(givenPlayerData, playerData);
            Assert.Equal(givenPlayerData.ToJson(), playerData.ToJson());
        }
    }
}
