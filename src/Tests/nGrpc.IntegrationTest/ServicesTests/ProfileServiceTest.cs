using nGrpc.ProfileService;
using System;
using System.Threading.Tasks;
using Xunit;

namespace nGrpc.IntegrationTest.ServicesTests
{
    public class ProfileServiceTest : IntegrationTestBase
    {
        [Fact]
        public async Task RegisterAndLoginTest()
        {
            IProfile profile = GetServiceFromProvider<IProfile>();

            (int playerId, Guid secretKey) = await profile.Register();
            Guid sessionId = await profile.Login(playerId, secretKey);

            Assert.NotEqual(0, playerId);
            Assert.NotEqual(Guid.Empty, secretKey);
            Assert.NotEqual(Guid.Empty, sessionId);
        }
    }
}
