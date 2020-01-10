using nGrpc.Common;
using nGrpc.ServerCommon;
using System;
using System.Threading.Tasks;
using Dapper;

namespace nGrpc.ProfileService
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly IDBProvider _dbProvider;
        private readonly ITime _time;

        public ProfileRepository(
            IDBProvider dbProvider,
            ITime time)
        {
            _dbProvider = dbProvider;
            _time = time;
        }

        public async Task<(int playerId, Guid secretKey)> Register()
        {
            PlayerData playerData = new PlayerData
            {
                RegisterDate = _time.UTCTime,
                SecretKey = Guid.NewGuid()
            };

            using (var conn = _dbProvider.GetConnection())
            {
                await conn.OpenAsync();
                string query = "insert into players(data) values('{}'::jsonb) returning id;";
                int Id = await conn.ExecuteScalarAsync<int>(query);
                playerData.Id = Id;
                query = "update players set data = @PlayerData::jsonb where id = @Id;";
                await conn.ExecuteAsync(query, new
                {
                    PlayerData = playerData.ToJson(),
                    Id = Id
                });

                return (Id, playerData.SecretKey);
            }
        }

        public async Task<PlayerData> Login(int playerId, Guid secretKey)
        {
            using (var conn = _dbProvider.GetConnection())
            {
                await conn.OpenAsync();
                string query = @"
select data from players
where id = @Id and data->>'SecretKey' = @SecretKey;";
                string json = await conn.QueryFirstOrDefaultAsync<string>(query, new
                {
                    Id = playerId,
                    SecretKey = secretKey.ToString()
                });

                PlayerData playerData = json.ToObject<PlayerData>();
                return playerData;
            }
        }

        public async Task SavePlayerData(PlayerData playerData)
        {
            using (var conn = _dbProvider.GetConnection())
            {
                await conn.OpenAsync();
                string query = "update players set data=@Data::jsonb where id=@Id";
                await conn.ExecuteAsync(query, new { Id = playerData.Id, Data = playerData.ToJson() });
            }
        }
    }
}
