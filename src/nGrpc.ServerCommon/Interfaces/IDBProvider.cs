using System.Data.Common;

namespace nGrpc.ServerCommon
{
    public interface IDBProvider
    {
        string GetConnectionString();
        DbConnection GetConnection();
    }
}
