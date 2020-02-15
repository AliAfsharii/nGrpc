using Grpc.Core;
using Microsoft.Extensions.Logging;
using nGrpc.Common;
using nGrpc.Common.Descriptors;
using nGrpc.ServerCommon;
using System;
using System.Threading.Tasks;

namespace nGrpc.ProfileService
{
    public class ProfileGrpcService : IGrpcService
    {
        private readonly ILogger<ProfileGrpcService> _logger;
        private readonly IProfile _profile;

        public ProfileGrpcService(ILogger<ProfileGrpcService> logger,
            IProfile profile)
        {
            _logger = logger;
            _profile = profile;
        }

        public void AddRpcMethods(IGrpcBuilderAdapter grpcBuilder)
        {
            grpcBuilder.AddMethod(ProfileDescriptors.RegisterDesc, RegisterRPC);
            grpcBuilder.AddMethod(ProfileDescriptors.LoginDesc, LoginRPC);
            grpcBuilder.AddMethod(ProfileDescriptors.ChangeNameDesc, ChangeNameRPC);
            grpcBuilder.AddMethod(ProfileDescriptors.GetPlayerDataDesc, GetPlayerDataRPC);
        }

        public async Task<RegisterRes> RegisterRPC(RegisterReq req, ServerCallContext context)
        {
            (int playerId, Guid secretKey) = await _profile.Register();
            _logger.LogInformation("RegisterRPC, PlayerId:{pid}, SecretKey:{sk}", playerId, secretKey);

            return new RegisterRes
            {
                PlayerId = playerId,
                SecretKey = secretKey
            };
        }

        public async Task<LoginRes> LoginRPC(LoginReq req, ServerCallContext context)
        {
            int? pi = context.GetPlayerCredential()?.PlayerId;
            if (pi != null)
                throw new PlayerHasAlreadyLoginedException($"PlayerId:{pi.Value}");

            int playerId = req.PlayerId;
            Guid secretKey = req.SecretKey;

            Guid sessionId = await _profile.Login(playerId, secretKey);
            _logger.LogInformation("LoginRPC, PlayerId:{pid}, SecretKey:{sk}, SessionId:{si}", playerId, secretKey, sessionId);

            return new LoginRes
            {
                PlayerId = playerId,
                SessionId = sessionId
            };
        }

        public async Task<ChangeNameRes> ChangeNameRPC(ChangeNameReq req, ServerCallContext context)
        {
            int playerId = context.GetPlayerCredential().PlayerId;
            string newName = req.NewName;

            PlayerData playerData = await _profile.ChangeName(playerId, newName);
            _logger.LogInformation("ChangeNameRPC, PlayerId:{pi}, NewName:{nm}", playerId, newName);

            return new ChangeNameRes
            {
                PlayerData = playerData
            };
        }

        public async Task<GetPlayerDataRes> GetPlayerDataRPC(GetPlayerDataReq req, ServerCallContext context)
        {
            int playerId = context.GetPlayerCredential().PlayerId;

            PlayerData playerData = await _profile.GetPlayerData(playerId);
            _logger.LogInformation("GetPlayerDataRPC, PlayerId:{pi}", playerId);

            return new GetPlayerDataRes
            {
                PlayerData = playerData
            };
        }
    }
}
