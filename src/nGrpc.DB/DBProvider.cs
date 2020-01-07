using nGrpc.ServerCommon;
using Npgsql;
using System.Data.Common;
using System.Text;

namespace nGrpc.DB
{
    public class DBProvider : IDBProvider
    {
        private readonly string _connectionString;

        public DBProvider(DBConfigs dbConfigs)
        {
            string poolSizeKey = "Maximum Pool Size";
            string connectionTimeoutKey = "Connection Idle Lifetime";
            string cs = dbConfigs.ConnectionString;

            StringBuilder stringBuilder = new StringBuilder(cs);
            if (cs.Contains(poolSizeKey) == false)
                DbConnectionStringBuilder.AppendKeyValuePair(stringBuilder, poolSizeKey, "100");
            if (cs.Contains(connectionTimeoutKey) == false)
                DbConnectionStringBuilder.AppendKeyValuePair(stringBuilder, connectionTimeoutKey, "30");
            _connectionString = stringBuilder.ToString();
        }

        public DbConnection GetConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}
