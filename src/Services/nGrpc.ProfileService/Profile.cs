using nGrpc.Common;
using nGrpc.ServerCommon;
using System;
using System.Threading.Tasks;

namespace nGrpc.ProfileService
{
    public class Profile : IProfile
    {
        private readonly IProfileRepository _profileRepository;
        private readonly ISessionsManager _sessionsManager;

        public Profile(IProfileRepository profileRepository, ServerCommon.ISessionsManager sessionsManager)
        {
            _profileRepository = profileRepository;
            _sessionsManager = sessionsManager;
        }

        public async Task<(int playerId, Guid secretKey)> Register()
        {
            (int playerId, Guid secretKey) = await _profileRepository.Register();
            return (playerId, secretKey);
        }

        public async Task<Guid> Login(int playerId, Guid secretKey)
        {
            if (_sessionsManager.HasSessionBySecretKey(playerId, secretKey, out Guid sessionId) == false)
            {
                PlayerData playerData = await _profileRepository.Login(playerId, secretKey);
                if (playerData == null)
                    throw new LoginFailedException($"Login Failed for PlayerId:{playerId}, SecretKey:{secretKey}");
                sessionId = _sessionsManager.AddSession(playerData);
            }

            return sessionId;
        }
    }
}
