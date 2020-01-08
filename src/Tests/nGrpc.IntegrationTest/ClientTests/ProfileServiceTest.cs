using nGrpc.Client;
using System.Threading.Tasks;
using Xunit;

namespace nGrpc.IntegrationTest.ClientTests
{
    public class ProfileServiceTest : IntegrationTestBase
    {
        [Fact]
        public async Task test()
        {
            GrpcChannel channel = await TestUtils.GetNewChannel();
         
        }
    }
}
