using Grpc.Core;
using Microsoft.Extensions.Logging;
using nGrpc.Common.Descriptors;
using nGrpc.Common.Models;
using nGrpc.ServerCommon;
using System;
using System.Threading.Tasks;

namespace nGrpc.ProfileService
{
    public class ProfileGrpc : IGrpcService
    {
        private readonly ILogger<ProfileGrpc> _logger;
        private readonly IProfile _profile;

        public ProfileGrpc(ILogger<ProfileGrpc> logger,
            IProfile profile)
        {
            _logger = logger;
            _profile = profile;
        }

        public void AddRpcMethods(IGrpcBuilderAdapter grpcBuilder)
        {
            grpcBuilder.AddMethod(ProfileDescriptors.RegisterDesc, RegisterRPC);
        }

        public async Task<RegisterRes> RegisterRPC(RegisterReq req, ServerCallContext context)
        {
            (int playerId, Guid secretKey) = await _profile.Register();
            return new RegisterRes
            {
                PlayerId = playerId,
                SecretKey = secretKey
            };
        }
    }
}
