using System.Collections.Generic;

namespace nGrpc.ServerCommon
{
    public interface IDBMigrator
    {
        void MigrateDB(bool dropDatabase, List<string> assembliesNames, Dictionary<string, string> variables = null);
    }
}
