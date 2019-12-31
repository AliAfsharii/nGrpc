using System;

namespace nGrpc.DB
{
    public class BadSqlFileNameExceptions : Exception
    {
        public BadSqlFileNameExceptions()
        {
        }

        public BadSqlFileNameExceptions(string message) : base(message)
        {
        }
    }
}
