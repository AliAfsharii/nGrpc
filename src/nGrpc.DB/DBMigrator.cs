using DbUp;
using Microsoft.Extensions.Logging;
using nGrpc.ServerCommon;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace nGrpc.DB
{
    public class DBMigrator : IDBMigrator
    {
        private readonly ILogger<DBMigrator> _logger;
        private readonly string _connectionString;

        public DBMigrator(ILogger<DBMigrator> logger,
            DBConfigs dbConfigs)
        {
            _logger = logger;

            string poolSizeKey = "Maximum Pool Size";
            string connectionTimeoutKey = "Connection Idle Lifetime";
            string cs = dbConfigs.ConnectionString;

            StringBuilder stringBuilder = new StringBuilder(cs);
            if (cs.Contains(poolSizeKey) == false)
                NpgsqlConnectionStringBuilder.AppendKeyValuePair(stringBuilder, poolSizeKey, "100");
            if (cs.Contains(connectionTimeoutKey) == false)
                NpgsqlConnectionStringBuilder.AppendKeyValuePair(stringBuilder, connectionTimeoutKey, "30");

            _connectionString = stringBuilder.ToString();
        }

        public void MigrateDB(bool dropDatabase, List<string> assembliesNames, Dictionary<string, string> variables = null)
        {
            if (dropDatabase == true)
            {
                _logger.LogInformation("Database is dropping...");

                var csb = new NpgsqlConnectionStringBuilder(_connectionString);
                var dbName = csb.Database;
                csb.Database = "postgres";

                using (NpgsqlConnection con = new NpgsqlConnection(csb.ConnectionString))
                {
                    con.Open();
                    string query = $@"SELECT pg_terminate_backend(pid)
                                    FROM pg_stat_activity
                                    WHERE datname = '{dbName}';
                                    DROP DATABASE IF EXISTS ""{ dbName}"";";

                    using (var command = new NpgsqlCommand(query, con))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                _logger.LogInformation("Database Dropped.");
            }

            EnsureDatabase.For.PostgresqlDatabase(_connectionString);

            Assembly[] allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assembly[] assemblies = allAssemblies.Where(n => assembliesNames.Contains(n.GetName().Name)).ToArray();

            var upgrader =
                DeployChanges.To
                    .PostgresqlDatabase(_connectionString)
                    .WithVariables(variables == null ? new Dictionary<string, string>() : variables)
                    .JournalToPostgresqlTable("", "dbmigration")
                    .WithTransactionPerScript()
                    .WithScriptsEmbeddedInAssemblies(assemblies)
                    .WithScriptNameComparer(new SqlFileNamesComparator())
                    .LogToConsole()
                    .Build();

            var result = upgrader.PerformUpgrade();

            if (result.Successful == false)
            {
                _logger.LogError(result.Error, "DataBase Migration Error");
                throw result.Error;
            }
            else
                _logger.LogInformation("DataBase Migration Succeeded!");
        }

        class SqlFileNamesComparator : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                var xs = x.Split('.');
                string xFileName = xs.Skip(xs.Length - 4).Take(4).Aggregate((n, m) => n + "." + m);

                var ys = y.Split('.');
                string yFileName = ys.Skip(ys.Length - 4).Take(4).Aggregate((n, m) => n + "." + m);

                return string.Compare(xFileName, yFileName);
            }
        }
    }
}
