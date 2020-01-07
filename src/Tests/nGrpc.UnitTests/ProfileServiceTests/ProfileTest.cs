using nGrpc.Common;
using nGrpc.ProfileService;
using nGrpc.ServerCommon;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace nGrpc.UnitTests.ProfileServiceTests
{
    public class ProfileTest
    {
        IProfile _profile;
        IProfileRepository _profileRepository;
        ISessionsManager _sessionsManager;

        public ProfileTest()
        {
            _profileRepository = Substitute.For<IProfileRepository>();
            _sessionsManager = Substitute.For<ISessionsManager>();
            _profile = new Profile(_profileRepository, _sessionsManager);
        }

        [Fact]
        public async Task GIVEN_Profile_WHEN_Call_Register_THEN_It_Should_Call_ProfileRepository_Register_Once_And_Return_Its_Returned_Values()
        {
            // given
            IProfile profile = _profile;
            IProfileRepository profileRepository = _profileRepository;
            int expectedPlayerId = 54634;
            Guid expectedSecretKey = Guid.NewGuid();
            profileRepository.Register().Returns((expectedPlayerId, expectedSecretKey));

            // when
            (int playerId, Guid secretKey) = await profile.Register();

            // then
            await profileRepository.Received(1).Register();
            Assert.Equal(expectedPlayerId, playerId);
            Assert.Equal(expectedSecretKey, secretKey);
        }

        [Fact]
        public async Task GIVEN_Profie_WHEN_Call_Login_For_A_Player_Without_Active_Session_THEN_It_Should_Create_A_Session_And_Return_SessionId()
        {
            // given
            IProfile profile = _profile;

            int playerId = 23145;
            Guid secretKey = Guid.NewGuid();

            IProfileRepository profileRepository = _profileRepository;
            PlayerData playerData = new PlayerData
            {
                Id = playerId,
                SecretKey = secretKey
            };
            profileRepository.Login(playerId, secretKey).Returns(playerData);

            ISessionsManager sessionsManager = _sessionsManager;
            Guid expectedSessionId = Guid.NewGuid();
            sessionsManager.AddSession(Arg.Any<PlayerData>()).Returns(expectedSessionId);

            // when
            Guid sessionId = await profile.Login(playerId, secretKey);

            // then
            await profileRepository.Received(1).Login(playerId, secretKey);
            sessionsManager.Received(1).AddSession(playerData);
            Assert.Equal(expectedSessionId, sessionId);
        }

        [Fact]
        public async Task GIVEN_Profie_WHEN_Call_Login_For_A_Player_With_Active_Session_THEN_It_Should_Return_SessionId()
        {
            // given
            IProfile profile = _profile;

            int playerId = 23145;
            Guid secretKey = Guid.NewGuid();

            IProfileRepository profileRepository = _profileRepository;
            ISessionsManager sessionsManager = _sessionsManager;
            sessionsManager.HasSessionBySecretKey(playerId, secretKey, out Guid expectedSessionId).Returns(true);

            // when
            Guid sessionId = await profile.Login(playerId, secretKey);

            // then
            await profileRepository.Received(0).Login(Arg.Any<int>(), Arg.Any<Guid>());
            sessionsManager.Received(0).AddSession(Arg.Any<PlayerData>());
            Assert.Equal(expectedSessionId, sessionId);
        }
    }
}
