namespace nGrpc.Sessions
{
    internal class Session
    {
        private PlayerData _playerData;

        internal Session(PlayerData playerData)
        {
            _playerData = playerData;
        }

        internal PlayerData GetPlayerData()
        {
            return _playerData;
        }
    }
}
