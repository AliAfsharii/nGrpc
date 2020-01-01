using System;

namespace nGrpc.DB
{
    public class BadSqlFileNameException : Exception
    {
        public BadSqlFileNameException()
        {
        }

        public BadSqlFileNameException(string message) : base(message)
        {
        }
    }
}
