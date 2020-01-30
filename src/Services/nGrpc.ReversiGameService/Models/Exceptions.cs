using System;

namespace nGrpc.ReversiGameService
{
    public class DiskOnWrongPositionException : Exception
    {
        public DiskOnWrongPositionException()
        {
        }

        public DiskOnWrongPositionException(string message) : base(message)
        {
        }
    }
}
