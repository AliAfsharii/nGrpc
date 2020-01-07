using System.Data.Common;

namespace nGrpc.ServerCommon
{
    public interface IDBProvider
    {
        DbConnection GetConnection();
    }
}
